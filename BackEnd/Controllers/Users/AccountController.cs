﻿using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Requests.Account;
using Models.People;
using Microsoft.AspNetCore.Authorization;
using Models.PublicAPI.Responses.People;

namespace BackEnd.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : AuthorizeController
    {
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly IUserRegisterTokens registerTokens;

        public AccountController(IMapper mapper,
            UserManager<User> userManager,
            IEmailSender emailSender,
            IUserRegisterTokens registerTokens) : base(userManager)
        {
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.registerTokens = registerTokens;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]AccountCreateRequest account)
        {
            if (!await registerTokens.IsCorrectRegisterToken(account.Email, account.AccessToken))
                return BadRequest("Incorrect access token");
            var user = mapper.Map<User>(account);
            user.EmailConfirmed = true;
            var result = await UserManager.CreateAsync(user, account.Password);
            if (result.Succeeded)
            {
                await registerTokens.RemoveToken(account.Email);
                await UserManager.AddClaimAsync(user, new System.Security.Claims.Claim("itlab", "user"));
            }

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<UserView>> EditUser([FromBody]AccountEditRequest editRequest)
        {
            var currentUser = await GetCurrentUser();
            mapper.Map(editRequest, currentUser);
            await UserManager.UpdateAsync(currentUser);
            return mapper.Map<UserView>(currentUser);
        }
        

        [HttpPut("password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await UserManager
                .ChangePasswordAsync(await GetCurrentUser(), request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("password/requestReset")]
        public async Task<ActionResult> ResetPassword([FromBody]RequestResetPasswordRequest request)
        {
            var userByMail = await UserManager.FindByEmailAsync(request.Email);
            if (userByMail == null)
                return NotFound();
            var token = await UserManager.GeneratePasswordResetTokenAsync(userByMail);
            await emailSender.SendResetPasswordEmail(request.Email, request.RedirectUrl, token);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("password/reset")]
        public async Task<ActionResult> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var userByMail = await UserManager.FindByEmailAsync(request.Email);
            if (userByMail == null)
                return NotFound();

            var result = await UserManager.ResetPasswordAsync(userByMail, request.Token, request.NewPassword);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }

    }
}