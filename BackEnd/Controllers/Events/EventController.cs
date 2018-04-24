using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.DataBaseLinks;
using Models.Events;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers.Events
{
    [Produces("application/json")]
    [Route("api/Event")]
    public class EventController : Controller
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EventTypeController> logger;
        private readonly IMapper mapper;

        public EventController(
            DataBaseContext dbContext,
            ILogger<EventTypeController> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ListResponse<EventPresent>> Get()
            => (await dbContext
            .Events
            .Include(e => e.EventEquipments)
            .ToListAsync())
            .Select(e => mapper.Map<EventPresent>(e))
            .ToList();

        [HttpGet("{id}")]
        public async Task<OneObjectResponse<Event>> GetAsync(Guid id)
            => await CheckAndGetEventAsync(id);


        [HttpPost]
        public async Task<OneObjectResponse<EventPresent>> PostAsync([FromBody]EventCreateRequest request)
        {
            var type = await CheckAndGetEventTypeAsync(request.EventTypeId);

            var newEvent = mapper.Map<Event>(request);
            await dbContext.Events.AddAsync(newEvent);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventPresent>(newEvent);
        }

        [HttpPut("addequipment")]
        public async Task<OneObjectResponse<EventPresent>> AddEquipments([FromBody]ChangeEquipmentRequest request)
        {
            var targetEvent = await dbContext
                .Events
                .Include(e => e.EventEquipments)
                .FirstOrDefaultAsync(e => e.Id == request.Id) ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);

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
            return mapper.Map<EventPresent>(targetEvent);
        }


        [HttpPut]
        public async Task<OneObjectResponse<EventPresent>> PutAsync(int id, [FromBody]EqiupmentEditRequest request)
        {
            var toEdit = await CheckAndGetEventAsync(request.Id);
            if (request.EventTypeId.HasValue)
                await CheckAndGetEventTypeAsync(request.EventTypeId.Value);

            mapper.Map(request, toEdit);

            await dbContext.SaveChangesAsync();

            return mapper.Map<EventPresent>(toEdit);
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> DeleteAsync([FromBody]IdRequest request)
        {
            var toDelete = await CheckAndGetEventAsync(request.Id);
            dbContext.Events.Remove(toDelete);
            await dbContext.SaveChangesAsync();
            return request.Id;
        }

        private async Task<Event> CheckAndGetEventAsync(Guid id)
            => await dbContext.Events.FindAsync(id)
              ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);

        private async Task<EventType> CheckAndGetEventTypeAsync(Guid typeId)
            => await dbContext.EventTypes.FindAsync(typeId)
                ?? throw ApiLogicException.Create(ResponseStatusCode.EventTypeNotFound);
    }
}
