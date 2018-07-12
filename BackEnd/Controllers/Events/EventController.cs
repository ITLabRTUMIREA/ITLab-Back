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
        public async Task<ListResponse<EventView>> Get(DateTime? begin, DateTime? end)
        {
            end = end == DateTime.MinValue ? DateTime.MaxValue : end;
            return await eventsManager
             .Events
			 .IfNotNull(begin, evnts => evnts.Where(e => e.Shifts.Min(c => c.BeginTime) >= begin))
			 .IfNotNull(end, evnts => evnts.Where(e => e.Shifts.Max(c => c.EndTime)<= end))
             .ProjectTo<EventView>()
             .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EventView>> GetAsync(Guid id)
            => mapper.Map<EventView>(await eventsManager
                .FindAsync(id));


        [HttpPost]
        public async Task<OneObjectResponse<EventView>> PostAsync([FromBody]EventCreateRequest request)
        {
            var newEvent = await eventsManager.AddAsync(request);
            return mapper.Map<EventView>(newEvent);
        }

        [HttpPut]
        public async Task<OneObjectResponse<EventView>> PutAsync([FromBody]EventEditRequest request)
        {
            var toEdit = await eventsManager.EditAsync(request);
            return mapper.Map<EventView>(toEdit);
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> DeleteAsync([FromBody]IdRequest request)
        {
            await eventsManager.DeleteAsync(request.Id);
            return request.Id;
        }
    }
}
