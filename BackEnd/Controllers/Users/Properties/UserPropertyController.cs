using System;
using Microsoft.AspNetCore.Identity;
using Models.People;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.DataBase;
using Models.PublicAPI.Responses.People.Properties;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using Models.PublicAPI.Requests.User.Properties.UserProperty;
using BackEnd.Services.UserProperties;
using Models.PublicAPI.Requests;
using System.Collections.Generic;
using AutoMapper;

namespace BackEnd.Controllers.Users.Properties
{
    /// <summary>
    /// Controller for manage user properties
    /// </summary>
    [Route("api/account/property")]
    public class UserPropertyController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly IUserPropertiesManager userPropertiesManager;
        private readonly IMapper mapper;

        public UserPropertyController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            IUserPropertiesManager userPropertiesManager,
            IMapper mapper
        ) : base(userManager)
        {
            this.dbContext = dbContext;
            this.userPropertiesManager = userPropertiesManager;
            this.mapper = mapper;
        }

        /// <summary>
        /// Return all users properties
        /// </summary>
        /// <returns>Users properties</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<UserPropertyView>>> GetAsync()
            => await dbContext
                .UserProperties
                .Where(u => u.UserId == UserId)
                .ProjectTo<UserPropertyView>(mapper.ConfigurationProvider)
                .ToListAsync();

        /// <summary>
        /// Edit property
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">When property type id is not found</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserPropertyView>> PutAsync(
            [FromBody] UserPropertyEditRequest request)
            => await (await userPropertiesManager.PutUserProperty(request, UserId))
                    .ProjectTo<UserPropertyView>(mapper.ConfigurationProvider)
                    .SingleAsync();

        /// <summary>
        /// Remove user property from user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Guid>> DeleteAsync(
            [FromBody] IdRequest request)
            => await userPropertiesManager.DeleteUserProperty(request.Id, UserId);

    }
}
