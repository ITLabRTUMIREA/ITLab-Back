using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Models.Settings;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.People;
using Models.People.UserProperties;
using Models.PublicAPI.Requests.User.Properties.HardProperties;
using Models.PublicAPI.Responses.People;

namespace BackEnd.Controllers.Users.Properties.HardProperties
{
    [Route("api/account/property/vk")]
    public class VkPropertyController : AuthorizeController
    {
        private readonly IUserRegisterTokens registerTokens;
        private readonly DataBaseContext dbContext;

        public VkPropertyController(
            UserManager<User> userManager,
            IUserRegisterTokens registerTokens,
            DataBaseContext dbContext) 
            : base(userManager)
        {
            this.registerTokens = registerTokens;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<string>> GetVkToken()
            => $"L:{await registerTokens.AddVkToken(UserId)}";

        // TODO use audience for VK
        [HttpPost]
        public async Task<ActionResult<UserView>> VerifyToken(
            [FromBody] VkVerifyRequest request)
        {
            var userId = await registerTokens.CheckVkToken(request.Token);
            if (userId == null)
                return BadRequest("Incorrect vk token");

            //TODO performance
            var vkPropType = await dbContext
                .UserPropertyTypes
                .SingleOrDefaultAsync(pt => pt.InternalName == UserPropertyNames.VKID.ToString());
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
                    UserId = userId.Value,
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
