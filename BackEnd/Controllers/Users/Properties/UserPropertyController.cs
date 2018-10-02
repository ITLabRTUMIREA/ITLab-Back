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

namespace BackEnd.Controllers.Users.Properties
{
    [Route("api/account/property")]
    public class UserPropertyController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;

        public UserPropertyController(
            UserManager<User> userManager,
            DataBaseContext dbContext) : base(userManager)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ListResponse<UserPropertyView>> GetAsync()
            => await dbContext
                .Users
                .Where(u => u.Id == UserId)
                .SelectMany(u => u.UserProperties)
                .ProjectTo<UserPropertyView>()
                .ToListAsync();


    }
}
