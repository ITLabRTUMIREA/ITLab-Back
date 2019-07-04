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

namespace BackEnd.Controllers.Events
{
    [Produces("application/json")]
    [Route("api/Event")]
    public class EventController : AuthorizeController
    {
        private readonly IEventsManager eventsManager;

        public EventController(
            UserManager<User> userManager,
            IEventsManager eventsManager,
            ILogger<EventTypeController> logger,
            IMapper mapper) : base(userManager)
        {
            this.eventsManager = eventsManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<CompactEventView>>> Get(DateTime? begin, DateTime? end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
                .Events
                .IfNotNull(begin, events => events.Where(e => e.EndTime >= begin))
                .IfNotNull(end, events => events.Where(e => e.BeginTime <= end))
                .OrderBy(cev => cev.BeginTime)
                .AttachUserId(UserId)
                .ProjectTo<CompactEventView>()
                .ToListAsync();
        }

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
            .ProjectTo<EventApplicationView>()
            .ToListAsync();

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpGet("wishers")]
        public async Task<ActionResult<List<WisherEventView>>> GetWishers()
        => await eventsManager
            .Events
            .SelectMany(e => e.Shifts)
            .SelectMany(s => s.Places)
            .SelectMany(p => p.PlaceUserEventRoles)
            .Where(pur => pur.UserStatus == UserStatus.Wisher)
            .ProjectTo<WisherEventView>()
            .ToListAsync();


        [HttpGet("{id}")]
        public async Task<ActionResult<EventView>> GetAsync(Guid id)
        {
            var targetEvent = await eventsManager
                           .Events
                           .ProjectTo<EventView > ()
                           .FirstOrDefaultAsync(ev => ev.Id == id);
            if (targetEvent == null) return NotFound();
            return targetEvent;
        }

        [Notify(NotifyType.EventNew)]
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost]
        public async Task<ActionResult<EventView>> PostAsync([FromBody] EventCreateRequest request)
            => await (await eventsManager.AddAsync(request))
                .ProjectTo<EventView>()
                .SingleAsync();

        [Notify(NotifyType.EventChange)]
        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPut]
        public async Task<ActionResult<EventView>> PutAsync([FromBody] EventEditRequest request)
            => await (await eventsManager.EditAsync(request))
                .ProjectTo<EventView>()
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
                    .ProjectTo<WisherEventView>()
                    .SingleOrDefaultAsync();

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("wish/{placeId:guid}/{userId:guid}/reject")]
        public async Task<ActionResult> RejectWish(Guid placeId, Guid userId)
        {
            await eventsManager.RejectWish(placeId, userId);
            return Ok();
    }
}
}