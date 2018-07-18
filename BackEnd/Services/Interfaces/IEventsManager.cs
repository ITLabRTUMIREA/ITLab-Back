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
        Task<Event> FindAsync(Guid id);
        Task<IQueryable<Event>> AddAsync(EventCreateRequest request);
        Task<IQueryable<Event>> EditAsync(EventEditRequest ev);
        Task DeleteAsync(Guid id);

    }
}
