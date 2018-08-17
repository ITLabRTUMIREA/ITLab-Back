using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Requests;
using Models;
using Models.PublicAPI.Requests.Account;
using Microsoft.Azure.KeyVault.Models;
using Models.PublicAPI.Responses;
using Models.People;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IEmailSender emailSender;
        private readonly IUserRegisterTokens registerTokens;

        public AccountController(IMapper mapper, 
            UserManager<User> userManager, 
            IEmailSender emailSender,
            IUserRegisterTokens registerTokens)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.registerTokens = registerTokens;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/{*emailToken}")]
        public async Task<ResponseBase> Get(string id, string emailToken)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.ConfirmEmailAsync(user, emailToken);
            if (result.Succeeded)
            {
                return ResponseStatusCode.OK;
            }
            else
            {
                return ResponseStatusCode.InvalidToken;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseBase> Post([FromBody]AccountCreateRequest account)
        {
            if (!await registerTokens.IsCorrectToken(account.Email, account.AccessToken))
                return ResponseStatusCode.IncorrectAccessToken;
            User user;
            user = mapper.Map<User>(account);
            var result = await userManager.CreateAsync(user, account.Password);
            if (result.Succeeded)
                await registerTokens.RemoveToken(account.Email);

            return ResponseStatusCode.OK;
        }
    }
}