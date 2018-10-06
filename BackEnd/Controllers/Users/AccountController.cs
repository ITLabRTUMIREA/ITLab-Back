using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Requests.Account;
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
        [HttpPost]
        public async Task<ResponseBase> Post([FromBody]AccountCreateRequest account)
        {
            if (!await registerTokens.IsCorrectToken(account.Email, account.AccessToken))
                return ResponseStatusCode.IncorrectAccessToken;
            var user = mapper.Map<User>(account);
            user.EmailConfirmed = true;
            var result = await UserManager.CreateAsync(user, account.Password);
            if (result.Succeeded)
                await registerTokens.RemoveToken(account.Email);

            return ResponseStatusCode.OK;
        }

        [HttpPut]
        public async Task<OneObjectResponse<UserView>> EditUser([FromBody]AccountEditRequest editRequest)
        {
            var currentUser = await GetCurrentUser();
            mapper.Map(editRequest, currentUser);
            await UserManager.UpdateAsync(currentUser);
            return mapper.Map<UserView>(currentUser);
        }
        

        [HttpPut("password")]
        public async Task<ResponseBase> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await UserManager
                .ChangePasswordAsync(await GetCurrentUser(), request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return ResponseBase.OK;

            throw ApiLogicException.Create(
                InputParameterIncorrectResponse.Create(result.Errors));
        }

        [AllowAnonymous]
        [HttpPost("password/requestReset")]
        public async Task<ResponseBase> ResetPassword([FromBody]RequestResetPasswordRequest request)
        {
            var userByMail = await UserManager.FindByEmailAsync(request.Email)
                                              ?? throw ResponseStatusCode.UserNotFound.ToApiException();
            var token = await UserManager.GeneratePasswordResetTokenAsync(userByMail);
            await emailSender.SendResetPasswordEmail(request.Email, request.RedirectUrl, token);
            return ResponseBase.OK;
        }

        [AllowAnonymous]
        [HttpPost("password/reset")]
        public async Task<ResponseBase> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var userByMail = await UserManager.FindByEmailAsync(request.Email)
                                              ?? throw ResponseStatusCode.UserNotFound.ToApiException();
            var result = await UserManager.ResetPasswordAsync(userByMail, request.Token, request.NewPassword);
            if (result.Succeeded)
                return ResponseBase.OK;

            throw ApiLogicException.Create(
                InputParameterIncorrectResponse.Create(result.Errors));
        }

    }
}