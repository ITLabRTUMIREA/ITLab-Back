using AutoMapper;
using BackEnd.Exceptions;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Roles;
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


        [RequireRole(RoleNames.CanEditRoles)]
        [HttpPost("{userId}/{roleId}")]
        public async Task<OneObjectResponse<bool>> AttachToRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString()) ?? throw ResponseStatusCode.NotFound.ToApiException();
            var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw ResponseStatusCode.NotFound.ToApiException();

            var addRoleResult = await userManager.AddToRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }
        [RequireRole(RoleNames.CanEditRoles)]
        [HttpDelete("{userId}/{roleId}")]
        public async Task<OneObjectResponse<bool>> DetachFromRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString()) ?? throw ResponseStatusCode.NotFound.ToApiException();
            var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw ResponseStatusCode.NotFound.ToApiException();

            var addRoleResult = await userManager.RemoveFromRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }

        [HttpGet]
        public async Task<ListResponse<Role>> GetAsync()
            => await roleManager.Roles.ToListAsync();

        [HttpPost]
        public async Task<OneObjectResponse<Role>> PostAsync([FromBody]RoleCreateRequest request)
        {
            var role = mapper.Map<Role>(request);
            var result = await roleManager.CreateAsync(role);
            return result.Succeeded ? role : throw ResponseStatusCode.FieldExist.ToApiException();
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> DeleteAsync([FromBody]IdRequest request)
        {
            var target = await roleManager.FindByIdAsync(request.Id.ToString()) ??
                throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            var result = await roleManager.DeleteAsync(target);
            return result.Succeeded ? target.Id : throw IdentityResultErrorResponse.Create(result.Errors, ResponseStatusCode.DeleteRoleError).ToApiException();
        }
    }
}
