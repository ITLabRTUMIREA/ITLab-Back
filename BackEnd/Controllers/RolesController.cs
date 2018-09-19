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
using BackEnd.Models.Roles;
using Models.People.Roles;

namespace BackEnd.Controllers
{
    [RequireRole(RoleNames.CanEditRoles)]
    [Route("api/roles")]
    public class RolesController : AuthorizeController
    {
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;

        public RolesController(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IMapper mapper) : base(userManager)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ListResponse<Role>> GetAsync()
            => await roleManager.Roles.ToListAsync();

        [HttpPost("{userId}/{roleId}")]
        public async Task<OneObjectResponse<bool>> AttachToRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();
            var user = await userManager.FindByIdAsync(userId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();

            var addRoleResult = await userManager.AddToRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }

        [HttpDelete("{userId}/{roleId}")]
        public async Task<OneObjectResponse<bool>> DetachFromRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();
            var user = await userManager.FindByIdAsync(userId.ToString()) ??
                       throw ResponseStatusCode.NotFound.ToApiException();

            var addRoleResult = await userManager.RemoveFromRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }
    }
}
