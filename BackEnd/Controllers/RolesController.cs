using AutoMapper;
using BackEnd.Exceptions;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Exceptions;
using Models.PublicAPI.Responses.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Models.Roles;
using Extensions;
using Models.People.Roles;
using Models.PublicAPI.Responses.People;

namespace BackEnd.Controllers
{
    [RequireRole(RoleNames.CanEditRoles)]
    [Route("api/roles")]
    public class RolesController : AuthorizeController
    {
        private readonly RoleManager<Role> roleManager;
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;

        public RolesController(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            DataBaseContext dbContext,
            IMapper mapper) : base(userManager)
        {
            this.roleManager = roleManager;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        [HttpGet("{userId:guid?}")]
        public async Task<ListResponse<RoleView>> GetAsync(Guid? userId)
            => await dbContext
                .Roles
                .If(userId.HasValue, rls =>
                    rls.Variable(out var roleNames,
                            () => UserManager.GetRolesAsync(GetUser(userId).Result).Result)
                        .Where(r => roleNames.Contains(r.Name)))
                .ProjectTo<RoleView>()
                .ToListAsync();

        [HttpPost("{userId}/{roleId}")]
        public async Task<OneObjectResponse<bool>> AttachToRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();
            var user = await UserManager.FindByIdAsync(userId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();

            var addRoleResult = await UserManager.AddToRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }

        [HttpDelete("{userId}/{roleId}")]
        public async Task<OneObjectResponse<bool>> DetachFromRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();
            var user = await UserManager.FindByIdAsync(userId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();

            var addRoleResult = await UserManager.RemoveFromRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }
    }
}
