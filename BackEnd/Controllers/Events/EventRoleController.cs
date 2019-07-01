using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper mapper;

        public EventRoleController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            IMapper mapper) : base(userManager)
        {
            this.dbContext = dbContext;
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

        [RequireRole(RoleNames.CanDeleteEventRole)]
        [HttpDelete]
        public async Task<ActionResult<EventRoleView>> Delete([FromBody] IdRequest request)
        {
            var target = await dbContext.EventRoles.SingleOrDefaultAsync(er => er.Id == request.Id);
            if (target == null)
                return NotFound();
            dbContext.Remove(target);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventRoleView>(target);
        }
    }
}
