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

        public AccountController(IMapper mapper, 
            UserManager<User> userManager, 
            IEmailSender emailSender)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/{*token}")]
        public async Task<ResponseBase> Get(string id, string token)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.ConfirmEmailAsync(user, token);
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
            User user;
            user = mapper.Map<User>(account);
            var result = await userManager.CreateAsync(user, account.Password);

            //result = await userManager.AddToRoleAsync(user, "User");
            //var token = userManager.GenerateEmailConfirmationTokenAsync(user);
            //var url = $"http://localhost:5000/api/Account/{user.Id}/{token}";
            //await emailSender.SendEmailConfirm(account.Email, url);

            return ResponseStatusCode.OK;
        }
    }
}