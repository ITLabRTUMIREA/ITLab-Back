using Microsoft.AspNetCore.Identity;
using Models.People;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Responses.People.Properties;
using BackEnd.DataBase;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI.Requests.User.Properties.UserPropertyType;
using System.Threading.Tasks;
using AutoMapper;
using Models.People.UserProperties;
using System.Collections.Generic;
using System;
using BackEnd.Models.Roles;
using Models.People.Roles;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace BackEnd.Controllers.Users.Properties
{
    [Route("api/account/property/type")]
    public class UserPropertyTypeController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly ILogger<UserPropertyTypeController> logger;
        private readonly IMapper mapper;

        public UserPropertyTypeController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            ILogger<UserPropertyTypeController> logger,
            IMapper mapper) : base(userManager)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns all user property types
        /// </summary>
        /// <returns>List of user property types</returns>
        [HttpGet]
        public async Task<ActionResult<List<UserPropertyTypeView>>> GetAsync()
            => await dbContext
                .UserPropertyTypes
                .ProjectTo<UserPropertyTypeView>(mapper.ConfigurationProvider)
                .ToListAsync();


        /// <summary>
        /// Returns one user property type by id
        /// </summary>
        /// <param name="id">Id of user property type</param>
        /// <returns>User property type</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not found item</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserPropertyTypeView>> GetAsync(Guid id)
        {
            var targetType = await dbContext.UserPropertyTypes
                           .Where(upt => upt.Id == id)
                           .ProjectTo<UserPropertyTypeView>(mapper.ConfigurationProvider)
                           .SingleOrDefaultAsync();
            if (targetType == null)
                return NotFound();
            return targetType;
        }

        /// <summary>
        /// Add new property type
        /// </summary>
        /// <param name="request">Data with needed user property type</param>
        /// <returns>Created user  property type</returns>
        /// <response code="201">Created new user property tye</response>
        /// <response code="400">Can't create user property type</response>
        /// <response code="409">Name of wanted type exists</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [HttpPost]
        [RequireRole(RoleNames.CanEditUserPropertyTypes)]
        public async Task<ActionResult<UserPropertyTypeView>> PostAsync(
            [FromBody]UserPropertyTypeCreateRequest request)
        {
            if (await dbContext.UserPropertyTypes.AnyAsync(upt => upt.PublicName == request.Title))
                return Conflict("Field exists");
            var newType = mapper.Map<UserPropertyType>(request);
            await dbContext.AddAsync(newType);
            var modifiedCount = await dbContext.SaveChangesAsync();
            if (modifiedCount == 0)
                return BadRequest("Can't add property type");
            return CreatedAtAction(nameof(GetAsync), new { id = newType.Id }, mapper.Map<UserPropertyTypeView>(newType));
        }

        /// <summary>
        /// Edit user property type
        /// </summary>
        /// <param name="id">Id of property type to delete</param>
        /// <returns>Id of deleted type</returns>
        /// <response code="200">Success editing</response>
        /// <response code="400">Can't edit user property type</response>
        /// <response code="404">Not fount user property type</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPut("{id:guid}")]
        //[RequireRole(RoleNames.CanEditUserPropertyTypes)]
        public async Task<ActionResult<Guid>> PutAsync(Guid id, [FromBody] UserPropertyTypeEditRequest request)
        {
            var targetUserPropertyType = await dbContext.UserPropertyTypes
                .FindAsync(id);
            if (targetUserPropertyType == null)
                return NotFound();

            mapper.Map(request, targetUserPropertyType);

            try
            {
                var modiciedCount = await dbContext.SaveChangesAsync();
                if (modiciedCount == 0)
                    return BadRequest("Can't delete user property type");
            }
            catch (DbUpdateException dbException)
            {
                logger.LogWarning(dbException, "Error while deleting user property type");
                return BadRequest("Can't delete user property type");
            }
            return id;
        }

        /// <summary>
        /// Delete user property type
        /// </summary>
        /// <param name="id">Id of property type to delete</param>
        /// <returns>Id of deleted type</returns>
        /// <response code="200">Success deleting</response>
        /// <response code="400">Can't delete user property type</response>
        /// <response code="404">Not fount user property type</response>
        /// <response code="409">Can't delete locked type</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("{id:guid}")]
        [RequireRole(RoleNames.CanEditUserPropertyTypes)]
        public async Task<ActionResult<Guid>> DeleteAsync(Guid id)
        {
            var targetUserPropertyType = await dbContext.UserPropertyTypes
                .FindAsync(id);
            if (targetUserPropertyType == null)
                return NotFound();
            if (Enum.IsDefined(typeof(UserPropertyNames), targetUserPropertyType.InternalName))
                return Conflict("Can't delete locked type");
            dbContext.UserPropertyTypes.Remove(targetUserPropertyType);
            try
            {
                var modiciedCount = await dbContext.SaveChangesAsync();
                if (modiciedCount == 0)
                    return BadRequest("Can't delete user property type");
            }
            catch (DbUpdateException dbException)
            {
                logger.LogWarning(dbException, "Error while deleting user property type");
                return BadRequest("Can't delete user property type");
            }
            return id;
        }
    }
}
