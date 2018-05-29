using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DataBase;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Newtonsoft.Json;
using Extensions;

namespace BackEnd.Controllers.Users
{
    [Route("api/user/settings")]
    public class UserSettingsController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;

        public UserSettingsController(UserManager<User> userManager,
                              DataBaseContext dbContext) : base(userManager)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ListResponse<UserSetting>> GetSettingsAsync()
        => await dbContext
                .UserSettings
                .Where(s => s.UserId == UserId)
                .ToListAsync();

        [HttpGet("{settingName}")]
        public async Task<OneObjectResponse<object>> GetSettingAsync(string settingName)
            => (await dbContext
                .UserSettings
                .Where(s => s.UserId == UserId)
                .Where(s => s.Title == settingName)
                .SingleOrDefaultAsync())
                ?.Value.ParseToJson()
                ?? throw ResponseStatusCode.NotFound.ToApiException();

        [HttpPost("{settingName}")]
        public async Task<OneObjectResponse<object>> SetSettingAsync(
            string settingName,
            [FromBody] object settingsValue)
        {
            var current = await dbContext
                  .UserSettings
                  .Where(s => s.UserId == UserId)
                  .Where(s => s.Title == settingName)
                  .SingleOrDefaultAsync();
            if (current == null)
            {
                current = new UserSetting { UserId = UserId, Title = settingName };
                dbContext.UserSettings.Add(current);
            }
            current.Value = JsonConvert.SerializeObject(settingsValue);
            await dbContext.SaveChangesAsync();
            return settingsValue;
        }
        [HttpDelete("{settingName}")]
        public async Task<OneObjectResponse<object>> DeleteSettingAsync(
            string settingName)
        {
            var current = await dbContext
                  .UserSettings
                  .Where(s => s.UserId == UserId)
                  .Where(s => s.Title == settingName)
                  .SingleOrDefaultAsync();
            if (current == null)
            {
                return null;
            }
            dbContext.UserSettings.Remove(current);
            await dbContext.SaveChangesAsync();
            return current.Value;
        }
    }
}
