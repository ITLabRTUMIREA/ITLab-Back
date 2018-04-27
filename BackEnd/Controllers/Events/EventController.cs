using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Services.Interfaces;
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
using AutoMapper.QueryableExtensions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers.Events
{
    [Produces("application/json")]
    [Route("api/Event")]
    public class EventController : Controller
    {
        private readonly IEventsManager eventsManager;

        private readonly ILogger<EventTypeController> logger;
        private readonly IMapper mapper;

        public EventController(
            IEventsManager eventManager,
            ILogger<EventTypeController> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventsManager = eventManager;
        }
        [HttpGet]
        public async Task<ListResponse<EventPresent>> Get()
            => await eventsManager
            .Events
            .ProjectTo<EventPresent>()
            .ToListAsync();

        [HttpGet("{id}")]
        public async Task<OneObjectResponse<Event>> GetAsync(Guid id)
            => await eventsManager.FindAsync(id);


        [HttpPost]
        public async Task<OneObjectResponse<EventPresent>> PostAsync([FromBody]EventCreateRequest request)
        {
            var newEvent = await eventsManager.AddAsync(request);
            return mapper.Map<EventPresent>(newEvent);
        }

        [HttpPut("addequipment")]
        public async Task<OneObjectResponse<EventPresent>> AddEquipments([FromBody]ChangeEquipmentRequest request)
        {
            var targetEvent = await eventsManager.AddEquipmentAsync(request);
            return mapper.Map<EventPresent>(targetEvent);
        }


        [HttpPut]
        public async Task<OneObjectResponse<EventPresent>> PutAsync([FromBody]EventEditRequest request)
        {
            var toEdit = await eventsManager.EditAsync(request);
            return mapper.Map<EventPresent>(toEdit);
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> DeleteAsync([FromBody]IdRequest request)
        {
            await eventsManager.DeleteAsync(request.Id);
            return request.Id;
        }
    }
}
