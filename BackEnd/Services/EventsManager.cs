using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Extensions;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DataBaseLinks;
using Models.Events;
using Models.PublicAPI.Requests.Events.Event;
using Models.PublicAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;

namespace BackEnd.Services
{
    public class EventsManager : IEventsManager
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;

        public EventsManager(
            DataBaseContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public IQueryable<Event> Events => dbContext.Events;

        public Task<Event> FindAsync(Guid id)
            => CheckAndGetEventAsync(id);

        public async Task<IQueryable<Event>> AddAsync(EventCreateRequest request)
        {
            await CheckAndGetEventTypeAsync(request.EventTypeId);
            var newEvent = mapper.Map<Event>(request);

            await dbContext.Events.AddAsync(newEvent);
            await dbContext.SaveChangesAsync();
            return dbContext.Events.Where(ev => ev.Id == newEvent.Id);
        }


        public async Task<IQueryable<Event>> EditAsync(EventEditRequest request)
        {
            var toEdit = await CheckAndGetEventAsync(request.Id);
            if (request.EventTypeId.HasValue)
                await CheckAndGetEventTypeAsync(request.EventTypeId.Value);

            mapper.Map(request, toEdit);
            if (toEdit.Shifts?.Count < 1)
                throw ResponseStatusCode.LastShift.ToApiException();
            await dbContext.SaveChangesAsync();
            return dbContext.Events.Where(ev => ev.Id == toEdit.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await CheckAndGetEventAsync(id);
            dbContext.Events.Remove(toDelete);
            await dbContext.SaveChangesAsync();
        }

        private async Task<EventType> CheckAndGetEventTypeAsync(Guid typeId)
            => await dbContext.EventTypes.FindAsync(typeId)
               ?? throw ApiLogicException.Create(ResponseStatusCode.EventTypeNotFound);

        private async Task<Event> CheckAndGetEventAsync(Guid id)
            => await dbContext.Events
                   .Include(e => e.EventType)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceEquipments)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceUserRoles)
                   .ThenInclude(pur => pur.Role)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceUserRoles)
                   .ThenInclude(pur => pur.User)
                   .FirstOrDefaultAsync(e => e.Id == id)
               ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
    }
}