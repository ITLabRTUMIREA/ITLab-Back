using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Extensions;
using BackEnd.Models.Settings;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.People;
using Models.People.UserProperties;
using Models.PublicAPI.Requests.User.Properties.HardProperties;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.People;

namespace BackEnd.Controllers.Users.Properties.HardProperties
{
    [Route("api/account/property/vk")]
    public class VkPropertyController : AuthorizeController
    {
        private readonly IUserRegisterTokens registerTokens;
        private readonly IOptions<NotifierSettings> config;
        private readonly DataBaseContext dbContext;

        public VkPropertyController(
            UserManager<User> userManager,
            IUserRegisterTokens registerTokens,
            IOptions<NotifierSettings> config,
            DataBaseContext dbContext) 
            : base(userManager)
        {
            this.registerTokens = registerTokens;
            this.config = config;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<OneObjectResponse<string>> GetVkToken()
            => $"L:{await registerTokens.AddVkToken(UserId)}";

        [HttpPost]
        [AllowAnonymous]
        public async Task<OneObjectResponse<UserView>> VerifyToken(
            [FromBody] VkVerifyRequest request)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var accessToken))
                throw ResponseStatusCode.Unauthorized.ToApiException();
            if (accessToken != config.Value.AccessToken)
                throw ResponseStatusCode.Forbidden.ToApiException();
            var userId = await registerTokens.CheckVkToken(request.Token)
                       ?? throw ResponseStatusCode.IncorrectVkToken.ToApiException();


            //TODO performance
            var vkPropType = await dbContext
                .UserPropertyTypes
                .SingleOrDefaultAsync(pt => pt.Name == UserPropertyNames.VKID.ToString());
            var vkProp = await dbContext
                .UserProperties
                .Include(u => u.UserPropertyType)
                .SingleOrDefaultAsync(up => up.UserId == userId);

            if (vkProp != null)
                vkProp.Value = request.VkId.ToString();
            else
            {    vkProp = new UserProperty
                {
                    UserPropertyTypeId = vkPropType.Id,
                    UserId = userId,
                    Value = request.VkId.ToString(),
                    Status = UserPropertyStatus.Confirmed
                };
                dbContext.Add(vkProp);
            }

            await dbContext.SaveChangesAsync();

            return await dbContext
                .Users
                .Where(u => u.Id == userId)
                .ProjectTo<UserView>()
                .SingleOrDefaultAsync();
        }
    }
}
