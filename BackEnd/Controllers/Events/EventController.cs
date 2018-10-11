using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using AutoMapper.QueryableExtensions;
using Extensions;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
using Models.PublicAPI.Responses;
using Microsoft.AspNetCore.Identity;
using Models.People;
using BackEnd.Extensions;
using BackEnd.Models.Roles;
using Models.People.Roles;
using Models.PublicAPI.Responses.Event.Invitations;

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
        public async Task<ListResponse<CompactEventView>> Get(DateTime? begin, DateTime? end)
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
        public async Task<ListResponse<EventApplicationView>> GetInvites(string requestType)
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

        [HttpGet("wishers")]
        public async Task<ListResponse<WisherEventView>> GetWishers()
        => await eventsManager
            .Events
            .SelectMany(e => e.Shifts)
            .SelectMany(s => s.Places)
            .SelectMany(p => p.PlaceUserEventRoles)
            .Where(pur => pur.UserStatus == UserStatus.Wisher)
            .ProjectTo<WisherEventView>()
            .ToListAsync();


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EventView>> GetAsync(Guid id)
            => await eventsManager
                .Events
                .ProjectTo<EventView>()
                .FirstOrDefaultAsync(ev => ev.Id == id)
                ?? throw ResponseStatusCode.NotFound.ToApiException();

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost]
        public async Task<OneObjectResponse<EventView>> PostAsync([FromBody] EventCreateRequest request)
            => await (await eventsManager.AddAsync(request))
                .ProjectTo<EventView>()
                .SingleAsync();

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPut]
        public async Task<OneObjectResponse<EventView>> PutAsync([FromBody] EventEditRequest request)
            => await (await eventsManager.EditAsync(request))
                .ProjectTo<EventView>()
                .SingleAsync();

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpDelete("{eventId:guid}")]
        public async Task<OneObjectResponse<Guid>> DeleteAsync(Guid eventId)
        {
            await eventsManager.DeleteAsync(eventId);
            return eventId;
        }

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("invitation/{placeId:guid}/{roleId:guid}/{userId:guid}")]
        public async Task<ResponseBase> Invite(Guid placeId, Guid roleId, Guid userId)
        {
            await eventsManager.InviteTo(placeId, roleId, userId);
            return ResponseStatusCode.OK;
        }

        [HttpPost("invitation/{placeId:guid}/accept")]
        public async Task<ResponseBase> AcceptInvite(Guid placeId)
        {
            await eventsManager.AcceptInvite(placeId, UserId);
            return ResponseStatusCode.OK;
        }


        [HttpPost("invitation/{placeId:guid}/reject")]
        public async Task<ResponseBase> RejectInvite(Guid placeId)
        {
            await eventsManager.RejectInvite(placeId, UserId);
            return ResponseStatusCode.OK;
        }

        [HttpPost("wish/{placeId:guid}/{roleId:guid}")]
        public async Task<ResponseBase> Wish(Guid placeId, Guid roleId)
        {
            await eventsManager.WishTo(UserId, roleId, placeId);
            return ResponseStatusCode.OK;
        }

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("wish/{placeId:guid}/{userId:guid}/accept")]
        public async Task<ResponseBase> AcceptWish(Guid placeId, Guid userId)
        {
            await eventsManager.AcceptWish(placeId, userId);
            return ResponseStatusCode.OK;
        }

        [RequireRole(RoleNames.CanEditEvent)]
        [HttpPost("wish/{placeId:guid}/{userId:guid}/reject")]
        public async Task<ResponseBase> RejectWish(Guid placeId, Guid userId)
        {
            await eventsManager.RejectWish(placeId, userId);
            return ResponseStatusCode.OK;
        }
    }
}