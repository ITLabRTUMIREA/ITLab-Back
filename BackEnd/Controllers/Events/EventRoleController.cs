using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Events.Roles;
using Models.People;
using Models.People.Roles;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.Event.Edit.Roles;
using Models.PublicAPI.Responses.Event;

namespace BackEnd.Controllers.Events
{
    [Route("api/EventRole")]
    public class EventRoleController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly ILogger<EventRoleController> logger;
        private readonly IMapper mapper;

        public EventRoleController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            ILogger<EventRoleController> logger,
            IMapper mapper) : base(userManager)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventRoleView>>> Get()
            => await dbContext
                .EventRoles
                .ProjectTo<EventRoleView>()
                .ToListAsync();

        [HttpPost]
        public async Task<ActionResult<EventRoleView>> Post([FromBody] EventRoleCreateRequest request)
        {
            if (await dbContext.EventRoles.AnyAsync(er => er.Title == request.Title))
                return Conflict("Event role name exist");
            var newEventRole = mapper.Map<EventRole>(request);
            dbContext.Add(newEventRole);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventRoleView>(newEventRole);
        }
        [HttpPut]
        public async Task<ActionResult<EventRoleView>> Put([FromBody] EventRoleEditRequest request)
        {
            var target = await dbContext.EventRoles.SingleOrDefaultAsync(er => er.Id == request.Id);
            if (target == null)
                return NotFound();
            target = mapper.Map(request, target);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventRoleView>(target);
        }

        /// <summary>
        /// Delete event role
        /// </summary>
        /// <param name="request">Id of deleting role</param>
        /// <returns>Id of deleted role</returns>
        /// <response code="200">Event role deleted</response>
        /// <response code="404">Not found event role</response>
        /// <response code="409">Can't delete event role</response>
        [RequireRole(RoleNames.CanDeleteEventRole)]
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<Guid>> Delete([FromBody] IdRequest request)
        {
            var target = await dbContext.EventRoles.SingleOrDefaultAsync(er => er.Id == request.Id);
            if (target == null)
                return NotFound();
            dbContext.Remove(target);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                logger.LogWarning(dbEx, "Error while deleting event role");
                return Conflict("Can't delete event role");
            }
            return request.Id;
        }
    }
}
