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
        [HttpGet("{begin?}/{end?}")]
        public async Task<ListResponse<EventPresent>> Get(DateTime begin, DateTime end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
             .Events
             .Where(e => e.BeginTime >= begin)
             .Where(e => e.BeginTime <= end)
             .ProjectTo<EventPresent>()
             .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EventPresent>> GetAsync(Guid id)
            => mapper.Map<EventPresent>(await eventsManager
                .FindAsync(id));


        [HttpPost]
        public async Task<OneObjectResponse<EventPresent>> PostAsync([FromBody]EventCreateRequest request)
        {
            var newEvent = await eventsManager.AddAsync(request);
            return mapper.Map<EventPresent>(newEvent);
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
