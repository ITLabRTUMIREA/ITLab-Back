using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        [HttpGet]
        public async Task<ListResponse<UserPresent>> GetAsync()
            => await userManager
                .Users
                .ProjectTo<UserPresent>()
                .ToListAsync();
        
        [HttpGet("{id}")]
        public async Task<OneObjectResponse<UserPresent>> GetAsync(Guid id)
        => await userManager
            .Users
            .ProjectTo<UserPresent>()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw ResponseStatusCode.NotFound.ToApiException();

    }
}