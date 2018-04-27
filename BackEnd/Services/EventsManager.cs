
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

        public IQueryable<Event> Events =>
            dbContext
            .Events
            .Include(e => e.EventEquipments);
        public Task<Event> FindAsync(Guid id)
            => CheckAndGetEventAsync(id);
        public async Task<Event> AddEquipmentAsync(ChangeEquipmentRequest request)
        {
            var targetEvent = await dbContext
                .Events
                .Include(e => e.EventEquipments)
                .FirstOrDefaultAsync(e => e.Id == request.Id) ?? throw ResponseStatusCode.NotFound.ToApiException();

            var targetEquipment = await dbContext
                .Equipments
                .Where(eq => request.EquipmentIds.Contains(eq.Id))
                .Where(eq => !dbContext
                    .Events
                    .Where(e => e.EndTime > targetEvent.BeginTime && e.BeginTime < targetEvent.EndTime)
                    .Any(e => e.EventEquipments.Any(eveq => eq.Id == eveq.EquipmentId)))
                .ToListAsync();

            if (targetEquipment.Count != request.EquipmentIds.Count)
                throw ApiLogicException.Create(ResponseStatusCode.IncorrectEquipmentIds);

            targetEvent
                .EventEquipments
                .AddRange
                    (targetEquipment
                    .Where(te => !targetEvent.EventEquipments.Select(et => et.EquipmentId).Contains(te.Id))
                     .Select(eq => EventEquipment.Create(targetEvent, eq))
                    );

            dbContext.SaveChanges();
            return targetEvent;
        }

        public async Task<Event> AddAsync(EventCreateRequest request)
        {
            var type = await CheckAndGetEventTypeAsync(request.EventTypeId);
            var newEvent = mapper.Map<Event>(request);
            await dbContext.Events.AddAsync(newEvent);
            await dbContext.SaveChangesAsync();
            return newEvent;
        }

        public Task<Event> AddPeople(Event ev)
        {
            throw new NotImplementedException();
        }

        public async Task<Event> EditAsync(EventEditRequest request)
        {
            var toEdit = await CheckAndGetEventAsync(request.Id);
            if (request.EventTypeId.HasValue)
                await CheckAndGetEventTypeAsync(request.EventTypeId.Value);

            mapper.Map(request, toEdit);
            await dbContext.SaveChangesAsync();
            return toEdit;
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
           => await dbContext.Events.FindAsync(id)
             ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);

        
    }
}
