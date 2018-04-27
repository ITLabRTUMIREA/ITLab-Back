using Models.Events;
using Models.PublicAPI.Requests.Events.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IEventsManager
    {
        IQueryable<Event> Events { get; }
        Task<Event> FindAsync(Guid id);
        Task<Event> AddAsync(EventCreateRequest request);
        Task<Event> EditAsync(EventEditRequest ev);

        Task DeleteAsync(Guid id);

        Task<Event> AddEquipmentAsync(ChangeEquipmentRequest ev);
        Task<Event> AddPeople(Event ev);

    }
}
