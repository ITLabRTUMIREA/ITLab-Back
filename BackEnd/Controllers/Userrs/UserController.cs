using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;
using Extensions;
using Newtonsoft.Json;

namespace BackEnd.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;

        public UserController(UserManager<User> userManager,
                              DataBaseContext dbContext) : base(userManager)
        {
            this.dbContext = dbContext;
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