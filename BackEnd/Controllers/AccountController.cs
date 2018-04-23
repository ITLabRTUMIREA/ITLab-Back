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

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly DataBaseContext dataBase;
        private readonly IEmailSender emailSender;

        public AccountController(IMapper mapper, 
            UserManager<User> userManager, 
            DataBaseContext dataBase, 
            IEmailSender emailSender)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.dataBase = dataBase;
            this.emailSender = emailSender;
        }

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

        [HttpPost]
        public async Task<ResponseBase> Post([FromBody]AccountCreateRequest account)
        {

            User user = mapper.Map<User>(account);
            var result = await userManager.CreateAsync(user, account.Password);
            result = await userManager.AddToRoleAsync(user, "User");

            var token = userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = $"http://localhost:5000/api/Account/{user.Id}/{token}";
            await emailSender.SendEmailConfirm(account.Email, url);

            return ResponseStatusCode.OK;
        }
    }
}