﻿using AutoMapper;
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
        public async Task<ListResponse<EventType>> GetAsync(string match, bool all = false)
           => await dbContext
                .EventTypes
                .OrderBy(et => et.Events.Count)
                .If(!all, evtypes => evtypes.Take(5))
                .IfNotNull(match, evtypes => 
                    evtypes.Where(et => et.Title.ToUpper().Contains(match.ToUpper())))
                .ToListAsync();


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EventType>> GetAsync(Guid id)
            => await CheckAndGetEquipmentTypeAsync(id);

        [HttpPost]
        public async Task<OneObjectResponse<EventType>> Post([FromBody]EventTypeCreateRequest request)
        {
            var EventType = mapper.Map<EventType>(request);
            var now = dbContext.EventTypes.FirstOrDefault(et => et.Title == request.Title);
            if (now != null)
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
            var added = await dbContext.EventTypes.AddAsync(EventType);
            await dbContext.SaveChangesAsync();
            return added.Entity;
        }


        [HttpPut]
        public async Task<OneObjectResponse<EventType>> Put([FromBody]EventTypeEditRequest request)
        {
            var now = await CheckAndGetEquipmentTypeAsync(request.Id);
            now.Title = request.Title;
            await dbContext.SaveChangesAsync();
            return now;
        }

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
