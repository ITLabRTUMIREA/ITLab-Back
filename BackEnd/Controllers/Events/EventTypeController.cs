using AutoMapper;
using BackEnd.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Events;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.EventType;
using System;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Models.PublicAPI.Responses.Event;
using AutoMapper.QueryableExtensions;
using BackEnd.Models.Roles;
using Models.People.Roles;
using System.Collections.Generic;

namespace BackEnd.Controllers.Events
{
    [Produces("application/json")]
    [Route("api/EventType")]
    public class EventTypeController : Controller
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EventTypeController> logger;
        private readonly IMapper mapper;

        public EventTypeController(
            DataBaseContext dbContext,
            ILogger<EventTypeController> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<EventTypeView>>> GetAsync(string match, bool all = false)
           => await dbContext
                .EventTypes
                .OrderBy(et => et.Events.Count)
                .IfNotNull(match, types =>
                    types.Where(et => et.Title.ToUpper().Contains(match.ToUpper())))
                .If(!all, types => types.Take(5))
                .ProjectTo<EventTypeView>()
                .ToListAsync();


        [HttpGet("{id}")]
        public async Task<ActionResult<EventTypeView>> GetAsync(Guid id)
        {
            var et = await dbContext.EventTypes.FindAsync(id);
            if (et == null)
                return NotFound();
            return mapper.Map<EventTypeView>(et);
        }

        [RequireRole(RoleNames.CanEditEventType)]
        [HttpPost]
        public async Task<ActionResult<EventTypeView>> Post([FromBody]EventTypeCreateRequest request)
        {
            var EventType = mapper.Map<EventType>(request);
            var now = dbContext.EventTypes.FirstOrDefault(et => et.Title == request.Title);
            if (now != null)
                return Conflict("Field exist");
            var added = await dbContext.EventTypes.AddAsync(EventType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventTypeView>(added.Entity);
        }

        [RequireRole(RoleNames.CanEditEventType)]
        [HttpPut]
        public async Task<ActionResult<EventTypeView>> Put([FromBody]EventTypeEditRequest request)
        {
            var now = await dbContext.EventTypes.FindAsync(request.Id);
            if (now == null)
                return NotFound();
            mapper.Map(request, now);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventTypeView>(now);
        }

        [RequireRole(RoleNames.CanEditEventType)]
        [HttpDelete]
        public async Task<ActionResult<Guid>> Delete([FromBody]IdRequest request)
        {
            var now = await dbContext.EventTypes.FindAsync(request.Id);
            if (now == null)
                return NotFound();
            dbContext.Remove(now);
            await dbContext.SaveChangesAsync();
            return now.Id;
        }
    }
}
