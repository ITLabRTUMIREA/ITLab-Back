using System.Linq;
using System.Threading.Tasks;
using BackEnd.DataBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.People;
using Newtonsoft.Json;
using Extensions;
using Models.PublicAPI.Responses.People;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using AutoMapper;

namespace BackEnd.Controllers.Users
{
    [Route("api/user/settings")]
    public class UserSettingsController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;

        public UserSettingsController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            IMapper mapper
        ) : base(userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<UserSettingPresent>>> GetSettingsAsync()
        => await dbContext
                .UserSettings
                .Where(s => s.UserId == UserId)
                .ProjectTo<UserSettingPresent>(mapper.ConfigurationProvider)
                .ToListAsync();

        [HttpGet("{settingName}")]
        public async Task<ActionResult<object>> GetSettingAsync(string settingName)
        {
            var settings = (await dbContext
                           .UserSettings
                           .Where(s => s.UserId == UserId)
                           .Where(s => s.Title == settingName)
                           .SingleOrDefaultAsync())
                           ?.Value.ParseToJson();
            if (settings == null)
                return NotFound();
            return settings;
        }

        [HttpPost("{settingName}")]
        public async Task<ActionResult<object>> SetSettingAsync(
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
        public async Task<ActionResult<object>> DeleteSettingAsync(
            string settingName)
        {
            var current = await dbContext
                  .UserSettings
                  .Where(s => s.UserId == UserId)
                  .Where(s => s.Title == settingName)
                  .SingleOrDefaultAsync();
            if (current == null)
                return NotFound();
            dbContext.UserSettings.Remove(current);
            await dbContext.SaveChangesAsync();
            return current.Value;
        }
    }
}
