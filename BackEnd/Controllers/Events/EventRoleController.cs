using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Extensions;
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
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;

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
        public async Task<ListResponse<EventRoleView>> Get()
            => await dbContext
                .EventRoles
                .ProjectTo<EventRoleView>()
                .ToListAsync();

        [HttpPost]
        public async Task<OneObjectResponse<EventRoleView>> Post([FromBody] EventRoleCreateRequest request)
        {
            if (await dbContext.EventRoles.AnyAsync(er => er.Title == request.Title))
                throw ResponseStatusCode.EventRoleNameExist.ToApiException();
            var newEventRole = mapper.Map<EventRole>(request);
            dbContext.Add(newEventRole);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventRoleView>(newEventRole);
        }
        [HttpPut]
        public async Task<OneObjectResponse<EventRoleView>> Put([FromBody] EventRoleEditRequest request)
        {
            var target = await dbContext.EventRoles.SingleOrDefaultAsync(er => er.Id == request.Id)
                ?? throw ResponseStatusCode.NotFound.ToApiException();
            target = mapper.Map(request, target);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventRoleView>(target);
        }

        [RequireRole(RoleNames.CanDeleteEventRole)]
        [HttpDelete]
        public async Task<OneObjectResponse<EventRoleView>> Delete([FromBody] IdRequest request)
        {
            var target = await dbContext.EventRoles.SingleOrDefaultAsync(er => er.Id == request.Id)
                         ?? throw ResponseStatusCode.NotFound.ToApiException();
            dbContext.Remove(target);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EventRoleView>(target);
        }
    }
}
