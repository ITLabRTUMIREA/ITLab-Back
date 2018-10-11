using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Extensions;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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

        public VkPropertyController(UserManager<User> userManager,
            IUserRegisterTokens registerTokens) 
            : base(userManager)
        {
            this.registerTokens = registerTokens;
        }
        [HttpGet]
        public async Task<OneObjectResponse<string>> GetVkToken()
            => await registerTokens.AddVkToken(UserId);

        [HttpPost]
        public async Task<Guid> VerifyToken([FromBody] VkVerifyRequest request)
        {
            var guid = await registerTokens.CheckVkToken(request.Token)
                       ?? throw ResponseStatusCode.IncorrectVkToken.ToApiException();

            return guid;
        }
    }
}
