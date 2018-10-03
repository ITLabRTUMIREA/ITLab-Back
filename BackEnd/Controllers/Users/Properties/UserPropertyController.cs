using System;
using Microsoft.AspNetCore.Identity;
using Models.People;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.DataBase;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People.Properties;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using Models.PublicAPI.Requests.User.Properties.UserProperty;
using BackEnd.Services.UserProperties;
using Models.PublicAPI.Requests;

namespace BackEnd.Controllers.Users.Properties
{
    [Route("api/account/property")]
    public class UserPropertyController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly IUserPropertiesManager userPropertiesManager;

        public UserPropertyController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            IUserPropertiesManager userPropertiesManager
        ) : base(userManager)
        {
            this.dbContext = dbContext;
            this.userPropertiesManager = userPropertiesManager;
        }

        [HttpGet]
        public async Task<ListResponse<UserPropertyView>> GetAsync()
            => await dbContext
                .Users
                .Where(u => u.Id == UserId)
                .SelectMany(u => u.UserProperties)
                .ProjectTo<UserPropertyView>()
                .ToListAsync();

        [HttpPut]
        public async Task<OneObjectResponse<UserPropertyView>> PutAsync(
            [FromBody] UserPropertyEditRequest request)
            => await (await userPropertiesManager.PutUserProperty(request, UserId))
                    .ProjectTo<UserPropertyView>()
                    .SingleAsync();

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> DeleteAsync(
            [FromBody] IdRequest request)
            => await userPropertiesManager.DeleteUserProperty(request.Id, UserId);

    }
}
