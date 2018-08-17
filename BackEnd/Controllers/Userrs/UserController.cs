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
using BackEnd.Services.Interfaces;
using Models.PublicAPI.Requests.User;

namespace BackEnd.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly IUserRegisterTokens registerTokens;
        private readonly IEmailSender emailSender;

        public UserController(UserManager<User> userManager,
                              DataBaseContext dbContext,
                              IUserRegisterTokens registerTokens,
                              IEmailSender emailSender) : base(userManager)
        {
            this.dbContext = dbContext;
            this.registerTokens = registerTokens;
            this.emailSender = emailSender;
        }
        [HttpGet]
        public async Task<ListResponse<UserView>> GetAsync(
            string email,
            string firstname,
            string lastname,
            string match,
            int count = 5)
            => await userManager
                .Users
                .ResetToDefault(m => true, ref match, match?.ToUpper())
                .IfNotNull(email, users => users.Where(u => u.Email.ToUpper().Contains(email.ToUpper())))
                .IfNotNull(firstname, users => users.Where(u => u.FirstName.ToUpper().Contains(firstname.ToUpper())))
                .IfNotNull(lastname, users => users.Where(u => u.LastName.ToUpper().Contains(lastname.ToUpper())))
                .IfNotNull(match, users => users.ForAll(match.Split(' '), (us2, matcher) =>  us2.Where(u => u.LastName.ToUpper().Contains(matcher)
                                                            || u.FirstName.ToUpper().Contains(matcher)
                                                            || u.Email.ToUpper().Contains(matcher)
                                                            || u.PhoneNumber.ToUpper().Contains(matcher))))
                .ResetToDefault(c => c <= 0, ref count, 5)
                .If(count > 0, users => users.Take(count))
                .ProjectTo<UserView>()
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<OneObjectResponse<UserView>> GetAsync(Guid id)
        => await userManager
            .Users
            .ProjectTo<UserView>()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw ResponseStatusCode.NotFound.ToApiException();

        [HttpPost]
        public async Task<ResponseBase> InviteUser([FromBody]InviteUserRequest inviteRequest)
        {
            var token = await registerTokens.AddRegisterToken(inviteRequest.Email);
            await emailSender.SendEmailConfirm(inviteRequest.Email, inviteRequest.RedirectUrl, token);
            return ResponseStatusCode.OK;
        }
    }
}