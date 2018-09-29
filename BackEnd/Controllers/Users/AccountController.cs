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
using Models.PublicAPI.Responses.People;
using Models.PublicAPI.Responses.General;
using BackEnd.Exceptions;
using Models.PublicAPI.Responses.Exceptions;

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

        [HttpPut]
        public async Task<OneObjectResponse<UserView>> EditUser([FromBody]AccountEditRequest editRequest)
        {
            var currentUser = await GetCurrentUser();
            mapper.Map(editRequest, currentUser);
            await userManager.UpdateAsync(currentUser);
            return mapper.Map<UserView>(currentUser);
        }

        [HttpPut("password")]
        public async Task<ResponseBase> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await userManager
                .ChangePasswordAsync(await GetCurrentUser(), request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return ResponseBase.OK;

            throw ApiLogicException.Create(
                InputParameterIncorrectResponse.Create(result.Errors));
        }

        [AllowAnonymous]
        [HttpPost("password/requestreset")]
        public async Task<ResponseBase> ResetPassword([FromBody]RequestResetPasswordRequest request)
        {
            var userByMail = await userManager.FindByEmailAsync(request.Email)
                                              ?? throw ResponseStatusCode.UserNotFound.ToApiException();
            var token = await userManager.GeneratePasswordResetTokenAsync(userByMail);
            await emailSender.SendResetPasswordEmail(request.Email, request.RedirectUrl, token);
            return ResponseBase.OK;
        }

        [AllowAnonymous]
        [HttpPost("password/reset")]
        public async Task<ResponseBase> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var userByMail = await userManager.FindByEmailAsync(request.Email)
                                              ?? throw ResponseStatusCode.UserNotFound.ToApiException();
            var result = await userManager.ResetPasswordAsync(userByMail, request.Token, request.NewPassword);
            if (result.Succeeded)
                return ResponseBase.OK;

            throw ApiLogicException.Create(
                InputParameterIncorrectResponse.Create(result.Errors));
        }

    }
}