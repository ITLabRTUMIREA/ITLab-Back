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
    public class AuthorizeController : Controller
    {
        protected readonly UserManager<User> userManager;

        public AuthorizeController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        protected Guid UserId => Guid.Parse(userManager.GetUserId(User));
        protected async Task<User> GetCurrentUser()
            => await userManager.FindByIdAsync(userManager.GetUserId(User));

        protected static T NotFound<T>() => throw ResponseStatusCode.NotFound.ToApiException();
    };
}