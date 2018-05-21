
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
            .OrderBy(e => e.BeginTime)
            .Include(e => e.EventEquipments);
        
        public Task<Event> FindAsync(Guid id)
            => CheckAndGetEventAsync(id);

        public async Task<Event> AddAsync(EventCreateRequest request)
        {
            var type = await CheckAndGetEventTypeAsync(request.EventTypeId);
            var newEvent = mapper.Map<Event>(request);
            request.Participants?.ForEach(pwr =>
            {
                if (pwr.BeginWork == null || pwr.EndWork == null)
                {
                    pwr.BeginWork = newEvent.BeginTime;
                    pwr.EndWork = newEvent.EndTime;
                    return;
                }
                if (pwr.BeginWork > newEvent.BeginTime || pwr.EndWork < newEvent.BeginTime)
                    throw ResponseStatusCode.IncorrectRequestData.ToApiException();
            });
            if (request.Participants?.Any() == true)
                if (await dbContext
                        .Users
                        .CountAsync(u => request.Participants.Any(p => p.Id == u.Id)) == request.Participants.Count)
                {
                    var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "PARTICIPANT");
                    newEvent.EventUsers = request.Participants.Select(userInfo => new EventUserRole
                    {
                        Event = newEvent,
                        UserId = userInfo.Id,
                        Role = role,
                        BeginWork = userInfo.BeginWork.Value,
                        EndWork = userInfo.EndWork.Value
                    }).ToList();
                }
                else
                    throw ResponseStatusCode.IncorrectUserIds.ToApiException();
            await UpdateEventEquipmentAsync(newEvent, request.Equipment);
            await dbContext.Events.AddAsync(newEvent);
            await dbContext.SaveChangesAsync();
            return newEvent;
        }
        private async Task UpdateEventEquipmentAsync(
            Event ev,
            List<Guid> add) => await UpdateEventEquipmentAsync(ev, add, new List<Guid>());

        public async Task<Event> EditAsync(EventEditRequest request)
        {
            var toEdit = await CheckAndGetEventAsync(request.Id);
            if (request.EventTypeId.HasValue)
                await CheckAndGetEventTypeAsync(request.EventTypeId.Value);

            mapper.Map(request, toEdit);

            await UpdateEventEquipmentAsync(toEdit, 
                request.AddEquipment,
                request.RemoveEquipment);
            await dbContext.SaveChangesAsync();
            return toEdit;
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await CheckAndGetEventAsync(id);
            dbContext.Events.Remove(toDelete);
            await dbContext.SaveChangesAsync();
        }

        private async Task UpdateEventEquipmentAsync(
            Event ev,
            List<Guid> add,
            List<Guid> remove)
        {
            add = add ?? new List<Guid>();
            remove = remove ?? new List<Guid>();
            var list = ev.EventEquipments ?? new List<EventEquipment>();
            var targetEquipmentIds = await dbContext
                .Equipments
                .Where(eq => add.Contains(eq.Id))
                .Where(eq => !dbContext
                    .Events
                    .Where(e => e.EndTime > ev.BeginTime && e.BeginTime < ev.EndTime)
                    .Any(e => e.EventEquipments.Any(eveq => eq.Id == eveq.EquipmentId)))
                .Select(eq => eq.Id)
                .ToListAsync();
            if (targetEquipmentIds.Count != add.Count)
                throw ResponseStatusCode.IncorrectEquipmentIds.ToApiException();
            list.AddRange(add.Where(g => !list.Any(eeq => eeq.EquipmentId == g)).Select(id => EventEquipment.Create(ev, id)));
            list.RemoveAll(eeq => remove.Contains(eeq.EquipmentId));
            ev.EventEquipments = list;
        }

        private async Task<EventType> CheckAndGetEventTypeAsync(Guid typeId)
            => await dbContext.EventTypes.FindAsync(typeId)
                ?? throw ApiLogicException.Create(ResponseStatusCode.EventTypeNotFound);

        private async Task<Event> CheckAndGetEventAsync(Guid id)
           => await dbContext.Events
                .Include(e => e.EventEquipments)
                .FirstOrDefaultAsync(e => e.Id == id)
             ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);


    }
}
