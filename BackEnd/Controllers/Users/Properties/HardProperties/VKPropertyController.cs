using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Extensions;
using BackEnd.Models.Settings;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models.People;
using Models.PublicAPI.Requests.User.Properties.HardProperties;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;

namespace BackEnd.Controllers.Users.Properties.HardProperties
{
    [Route("api/account/property/vk")]
    public class VkPropertyController : AuthorizeController
    {
        private readonly IUserRegisterTokens registerTokens;
        private readonly IOptions<NotifierSettings> config;

        public VkPropertyController(
            UserManager<User> userManager,
            IUserRegisterTokens registerTokens,
            IOptions<NotifierSettings> config) 
            : base(userManager)
        {
            this.registerTokens = registerTokens;
            this.config = config;
        }
        [HttpGet]
        public async Task<OneObjectResponse<string>> GetVkToken()
            => $"L:{await registerTokens.AddVkToken(UserId)}";

        [HttpPost]
        [AllowAnonymous]
        public async Task<OneObjectResponse<Guid>> VerifyToken(
            [FromBody] VkVerifyRequest request)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var accessToken))
                throw ResponseStatusCode.Unauthorized.ToApiException();
            if (accessToken != config.Value.AccessToken)
                throw ResponseStatusCode.Forbidden.ToApiException();
            var guid = await registerTokens.CheckVkToken(request.Token)
                       ?? throw ResponseStatusCode.IncorrectVkToken.ToApiException();

            return guid;
        }
    }
}
