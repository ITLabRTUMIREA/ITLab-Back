using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;
using AutoMapper.QueryableExtensions;
using Extensions;
using Models.Events;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
using Models.PublicAPI.Responses;
using Microsoft.AspNetCore.Identity;
using Models.People;
using BackEnd.Extensions;
using BackEnd.Models;
using Models.PublicAPI.Responses.Event.Invitations;

namespace BackEnd.Controllers.Events
{
    [Produces("application/json")]
    [Route("api/Event")]
    public class EventController : AuthorizeController
    {
        private readonly IEventsManager eventsManager;

        private readonly ILogger<EventTypeController> logger;
        private readonly IMapper mapper;

        public EventController(
            UserManager<User> userManager,
            IEventsManager eventManager,
            ILogger<EventTypeController> logger,
            IMapper mapper) : base(userManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventsManager = eventManager;
        }

        [HttpGet]
        public async Task<ListResponse<CompactEventView>> Get(DateTime? begin, DateTime? end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
                .Events
                .IfNotNull(begin, evnts => evnts.Where(e => e.BeginTime >= begin))
                .IfNotNull(end, evnts => evnts.Where(e => e.BeginTime <= end))
                .OrderBy(cev => cev.BeginTime)
                .AttachUserId(UserId)
                .ProjectTo<CompactEventView>()
                .ToListAsync();
        }

        [HttpGet("invitations")]
        public async Task<ListResponse<InvitationsEventView>> GeInvites()
            => await eventsManager
            .Events
            .Where(e => e
                   .Shifts
                   .SelectMany(s => s.Places)
                   .SelectMany(p => p.PlaceUserRoles)
                   .Any(pur => pur.UserId == UserId && 
                        (pur.UserStatus != UserStatus.Accepted )))
            .AttachUserId(UserId)
            .ProjectTo<InvitationsEventView>()
            .ToListAsync();

        [HttpGet("wishers")]
        public async Task<ListResponse<WisherEventView>> GetWishers()
        => await eventsManager
            .Events
            .SelectMany(e => e.Shifts)
            .SelectMany(s => s.Places)
            .SelectMany(p => p.PlaceUserRoles)
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


        [HttpPost]
        public async Task<OneObjectResponse<EventView>> PostAsync([FromBody] EventCreateRequest request)
        {
            var newEventQuerable = await eventsManager.AddAsync(request);
            return await newEventQuerable.ProjectTo<EventView>().SingleAsync();
        }

        [HttpPut]
        public async Task<OneObjectResponse<EventView>> PutAsync([FromBody] EventEditRequest request)
        => await (await eventsManager.EditAsync(request))
                .ProjectTo<EventView>()
                .SingleAsync();


        [HttpDelete("{eventId:guid}")]
        public async Task<OneObjectResponse<Guid>> DeleteAsync(Guid eventId)
        {
            await eventsManager.DeleteAsync(eventId);
            return eventId;
        }

        [HttpPost("wishTo/{placeId:guid}/{roleId:guid}")]
        public async Task<ResponseBase> WishTo(Guid placeId, Guid roleId)
        {
            await eventsManager.WishTo(UserId, roleId, placeId);
            return ResponseStatusCode.OK;
        }


        [HttpPost("{placeId:guid}/accept")]
        public async Task<ResponseBase> AcceptInvite(Guid placeId)
        {
            await eventsManager.AcceptInvite(placeId, UserId);
            return ResponseStatusCode.OK;
        }


        [HttpPost("{placeId:guid}/reject")]
        public async Task<ResponseBase> RejectInvite(Guid placeId)
        {
            await eventsManager.RejectInvite(placeId, UserId);
            return ResponseStatusCode.OK;
        }

        [HttpPost("acceptWish/{placeId:guid}/{userId:guid}")]
        public async Task<ResponseBase> AcceptWish(Guid placeId, Guid userId)
        {
            await eventsManager.AcceptWish(placeId, userId);
            return ResponseStatusCode.OK;
        }

        [HttpPost("rejectWish/{placeId:guid}/{userId:guid}")]
        public async Task<ResponseBase> RejectWish(Guid placeId, Guid userId)
        {
            await eventsManager.RejectWish(placeId, userId);
            return ResponseStatusCode.OK;
        }
    }
}