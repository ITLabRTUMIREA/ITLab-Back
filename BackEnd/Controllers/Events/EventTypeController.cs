using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Events;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.EventType;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Models.PublicAPI.Responses.Event;
using AutoMapper.QueryableExtensions;
using BackEnd.Models.Roles;
using Models.People.Roles;

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
        public async Task<ListResponse<EventTypeView>> GetAsync(string match, bool all = false)
           => await dbContext
                .EventTypes
                .OrderBy(et => et.Events.Count)
                .IfNotNull(match, types => 
                    types.Where(et => et.Title.ToUpper().Contains(match.ToUpper())))
                .If(!all, types => types.Take(5))
                .ProjectTo<EventTypeView>()
                .ToListAsync();


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EventTypeView>> GetAsync(Guid id)
            => mapper.Map<EventTypeView>(await CheckAndGetEquipmentTypeAsync(id));

        [RequireRole(RoleNames.CanEditEventType)]
        [HttpPost]
        public async Task<OneObjectResponse<EventTypeView>> Post([FromBody]EventTypeCreateRequest request)
        {
            var EventType = mapper.Map<EventType>(request);
            var now = dbContext.EventTypes.FirstOrDefault(et => et.Title == request.Title);
            if (now != null)
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
            var added = await dbContext.EventTypes.AddAsync(EventType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventTypeView>(added.Entity);
        }

        [RequireRole(RoleNames.CanEditEventType)]
        [HttpPut]
        public async Task<OneObjectResponse<EventTypeView>> Put([FromBody]EventTypeEditRequest request)
        {
            var now = await CheckAndGetEquipmentTypeAsync(request.Id);
            mapper.Map(request, now);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventTypeView>(now);
        }

        [RequireRole(RoleNames.CanEditEventType)]
        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> Delete([FromBody]IdRequest request)
        {
            var now = await CheckAndGetEquipmentTypeAsync(request.Id);
            dbContext.Remove(now);
            await dbContext.SaveChangesAsync();
            return now.Id;
        }

        private async Task<EventType> CheckAndGetEquipmentTypeAsync(Guid typeId)
            => await dbContext.EventTypes.FindAsync(typeId)
                ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
    }
}
