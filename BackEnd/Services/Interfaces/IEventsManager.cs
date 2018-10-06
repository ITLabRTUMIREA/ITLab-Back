using Models.Events;
using Models.PublicAPI.Requests.Events.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;

namespace BackEnd.Services.Interfaces
{
    public interface IEventsManager
    {
        IQueryable<Event> Events { get; }
        Task<IQueryable<Event>> AddAsync(EventCreateRequest request);
        Task<IQueryable<Event>> EditAsync(EventEditRequest ev);
        Task DeleteAsync(Guid id);
        Task WishTo(Guid userId, Guid eventRoleId, Guid placeId);
        Task InviteTo(Guid userId, Guid eventRoleId, Guid placeId);
        Task AcceptInvite(Guid placeId, Guid userId);
        Task RejectInvite(Guid placeId, Guid userId);
        Task AcceptWish(Guid placeId, Guid userId);
        Task RejectWish(Guid placeId, Guid userId);
    }
}
