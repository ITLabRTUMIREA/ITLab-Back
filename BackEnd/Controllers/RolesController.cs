using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
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


        [HttpGet()]
        public async Task<ActionResult<List<RoleView>>> GetAsync()
        {
            return await dbContext.Roles
                .ProjectTo<RoleView>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Get roles for specific user
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <returns>List of roles</returns>
        /// <response code="200">Success get roles</response>
        /// <response code="404">No roles for user</response>
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<RoleView>>> GetAsync(Guid userId)
        {
            var roles = await dbContext
                .Roles
                .ProjectTo<RoleView>(mapper.ConfigurationProvider)
                .ToListAsync();
            var roleIds = await dbContext.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync();
            if (roleIds.Count == 0)
                return NotFound();
            return roles.Where(r => roleIds.Contains(r.Id)).ToList();
        }

        [HttpPost("{userId}/{roleId}")]
        public async Task<ActionResult<bool>> AttachToRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return NotFound();
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();

            var addRoleResult = await UserManager.AddToRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }

        [HttpDelete("{userId}/{roleId}")]
        public async Task<ActionResult<bool>> DetachFromRole(Guid userId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return NotFound();
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();

            var addRoleResult = await UserManager.RemoveFromRoleAsync(user, role.Name);
            return addRoleResult.Succeeded;
        }
    }
}
