using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.People;
using Models.PublicAPI.Responses;

namespace BackEnd.Controllers
{
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        protected readonly UserManager<User> UserManager;

        public AuthorizeController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }
        protected Guid UserId => Guid.Parse(UserManager.GetUserId(User));
        protected async Task<User> GetCurrentUser()
            => await UserManager.FindByIdAsync(UserManager.GetUserId(User));
        protected async Task<User> GetUser(Guid? userId)
            => await UserManager.FindByIdAsync(userId.ToString());
    };
}