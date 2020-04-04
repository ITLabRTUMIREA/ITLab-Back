using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.Responses.Event;
using AutoMapper.QueryableExtensions;
using Extensions;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
using Microsoft.AspNetCore.Identity;
using Models.People;
using BackEnd.Extensions;
using BackEnd.Models.Roles;
using BackEnd.Services.Notify;
using Models.People.Roles;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event.Invitations;
using System.Collections.Generic;
using Models.PublicAPI.Responses.Event.CreateEdit;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices.WindowsRuntime;
using BackEnd.Models.Settings;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers.Events
{
    /// <summary>
    /// Controller about events
    /// </summary>
    [Route("api/Event")]
    public class EventController : AuthorizeController
    {
        private readonly IEventsManager eventsManager;
        private readonly IMapper mapper;

        public EventController(
            UserManager<User> userManager,
            IEventsManager eventsManager,
            ILogger<EventTypeController> logger,
            IMapper mapper) : base(userManager)
        {
            this.eventsManager = eventsManager;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get events list
        /// </summary>
        /// <param name="begin">Biggest end time. If not defined end time equals infinity</param>
        /// <param name="end">Smallest begin time. If not defined begin time equals zero</param>
        /// <returns>List of events</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Incorrect begin or end parameter format</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet]
        public async Task<ActionResult<List<CompactEventView>>> Get(DateTime? begin, DateTime? end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
                .Events
                .IfNotNull(begin, events => events.Where(e => e.EndTime >= begin))
                .IfNotNull(end, events => events.Where(e => e.BeginTime <= end))
                .OrderBy(cev => cev.BeginTime)
                .AttachUserId(mapper.ConfigurationProvider, UserId)
                .ProjectTo<CompactEventView>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Get events list
        /// </summary>
        /// <param name="begin">Biggest end time. If not defined end time equals infinity</param>
        /// <param name="end">Smallest begin time. If not defined begin time equals zero</param>
        /// <returns>List of events</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Incorrect begin or end parameter format</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("docsGen")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EventView>>> GetForDocsGenerator(
            [FromServices] IOptions<DocsGeneratorSettings> options,
            DateTime? begin,
            DateTime? end)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                return Unauthorized("Need documents generation secret string");
            }
            if (token != options.Value.AccessToken)
            {
                return Forbid("Invalid access token");
            }
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
                .Events
                .IfNotNull(begin, events => events.Where(e => e.EndTime >= begin))
                .IfNotNull(end, events => events.Where(e => e.BeginTime <= end))
                .OrderBy(cev => cev.BeginTime)
                .AttachUserId(mapper.ConfigurationProvider, UserId)
                .ProjectTo<EventView>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Get events ids list
        /// </summary>
        /// <param name="begin">Biggest end time. If not defined end time equals infinity</param>
        /// <param name="end">Smallest begin time. If not defined begin time equals zero</param>
        /// <returns>List of events</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Incorrect begin or end parameter format</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("ids")]
        public async Task<ActionResult<List<Guid>>> GetIds(DateTime? begin, DateTime? end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
                .Events
                .IfNotNull(begin, events => events.Where(e => e.EndTime >= begin))
                .IfNotNull(end, events => events.Where(e => e.BeginTime <= end))
                .Select(ev => ev.Id)
                .ToListAsync();
        }


        /// <summary>
        /// Get events list for specific user
        /// </summary>
        /// <param name="userId">Id of finding user</param>
        /// <param name="begin">Biggest end time. If not defined end time equals infinity</param>
        /// <param name="end">Smallest begin time. If not defined begin time equals zero</param>
        /// <returns>List of events</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Incorrect userId, begin or end parameter format</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("user/{userId:Guid}")]
        public async Task<ActionResult<List<UsersEventsView>>> GetUsersEvents(Guid userId, DateTime? begin, DateTime? end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
                .Events
                .SelectMany(e => e.Shifts)
                .IfNotNull(begin, events => events.Where(e => e.EndTime >= begin))
                .IfNotNull(end, events => events.Where(e => e.BeginTime <= end))
                .OrderBy(cev => cev.BeginTime)
                .SelectMany(s => s.Places)
                .SelectMany(p => p.PlaceUserEventRoles)
                .Where(puer => puer.UserId == userId && puer.UserStatus == UserStatus.Accepted)
                .ProjectTo<UsersEventsView>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Get invites or wishes of authorized user
        /// </summary>
        /// <param name="requestType">type of request, "wishes" for wishes, "invitations" for invitations</param>
        /// <returns>List of invites</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Success</response>
        [HttpGet("applications/{requestType}")]
        public async Task<ActionResult<List<EventApplicationView>>> GetInvites(string requestType)
            => await eventsManager
            .Events
            .Translate(requestType, out var userStatus,
                UserStatus.Unknown,
                ("wishes", UserStatus.Wisher),
                ("invitations", UserStatus.Invited))
            .SelectMany(ev => ev.Shifts)
            .SelectMany(s => s.Places)
            .SelectMany(p => p.PlaceUserEventRoles)
            .Where(pur => pur.UserId == UserId && pur.UserStatus == userStatus)
            .ProjectTo<EventApplicationView>(mapper.ConfigurationProvider)
            .ToListAsync();

        /// <summary>
        /// Get all wishers
        /// </summary>
        /// <returns>List of ishers</returns>
        /// <response code="200">Success</response>
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpGet("wishers")]
        public async Task<ActionResult<List<WisherEventView>>> GetWishers()
        => await eventsManager
            .Events
            .SelectMany(e => e.Shifts)
            .SelectMany(s => s.Places)
            .SelectMany(p => p.PlaceUserEventRoles)
            .Where(pur => pur.UserStatus == UserStatus.Wisher)
            .ProjectTo<WisherEventView>(mapper.ConfigurationProvider)
            .ToListAsync();

        /// <summary>
        /// Get event information
        /// </summary>
        /// <returns>Event with specific id</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Event id is not GUID</response>
        /// <response code="404">Event not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventView>> GetAsync(Guid id)
        {
            var targetEvent = await eventsManager
                           .Events
                           .ProjectTo<EventView>(mapper.ConfigurationProvider)
                           .FirstOrDefaultAsync(ev => ev.Id == id);
            if (targetEvent == null) return NotFound();
            return targetEvent;
        }


        


        /// <summary>
        /// Create new event
        /// </summary>
        /// <param name="request">New event information</param>
        /// <returns></returns>
        [Notify(NotifyType.EventNew)]
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost]
        public async Task<ActionResult<CreateEditEventView>> PostAsync([FromBody] EventCreateRequest request)
            => await (await eventsManager.AddAsync(request))
                .ProjectTo<CreateEditEventView>(mapper.ConfigurationProvider)
                .SingleAsync();

        /// <summary>
        /// Edit exists event
        /// </summary>
        /// <param name="request">New info about event</param>
        /// <returns></returns>
        [Notify(NotifyType.EventChange)]
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPut]
        public async Task<ActionResult<CreateEditEventView>> PutAsync([FromBody] EventEditRequest request)
            => await (await eventsManager.EditAsync(request))
                .ProjectTo<CreateEditEventView>(mapper.ConfigurationProvider)
                .SingleAsync();

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpDelete("{eventId:guid}")]
        public async Task<ActionResult<Guid>> DeleteAsync(Guid eventId)
        {
            await eventsManager.DeleteAsync(eventId);
            return eventId;
        }

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("invitation/{placeId:guid}/{roleId:guid}/{userId:guid}")]
        public async Task<ActionResult> Invite(Guid placeId, Guid roleId, Guid userId)
        {
            await eventsManager.InviteTo(placeId, roleId, userId);
            return Ok();
        }

        [HttpPost("invitation/{placeId:guid}/accept")]
        public async Task<ActionResult> AcceptInvite(Guid placeId)
        {
            await eventsManager.AcceptInvite(placeId, UserId);
            return Ok();
        }

        [HttpPost("invitation/{placeId:guid}/reject")]
        public async Task<ActionResult> RejectInvite(Guid placeId)
        {
            await eventsManager.RejectInvite(placeId, UserId);
            return Ok();
        }

        [HttpPost("wish/{placeId:guid}/{roleId:guid}")]
        public async Task<ActionResult> Wish(Guid placeId, Guid roleId)
        {
            await eventsManager.WishTo(UserId, roleId, placeId);
            return Ok();
        }

        [Notify(NotifyType.EventConfirm)]
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("wish/{placeId:guid}/{userId:guid}/accept")]
        public async Task<ActionResult<WisherEventView>> AcceptWish(Guid placeId, Guid userId)
            => await (await eventsManager
                    .AcceptWish(placeId, userId))
                    .ProjectTo<WisherEventView>(mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();

        [Notify(NotifyType.EventReject)]
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("wish/{placeId:guid}/{userId:guid}/reject")]
        public async Task<ActionResult> RejectWish(Guid placeId, Guid userId)
        {
            await eventsManager.RejectWish(placeId, userId);
            return Ok();
        }
    }
}