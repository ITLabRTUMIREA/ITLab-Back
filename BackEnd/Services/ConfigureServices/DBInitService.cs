using System;
using System.Threading.Tasks;
using WebApp.Configure.Models.Configure.Interfaces;
using Microsoft.Extensions.Options;
using BackEnd.Models.Settings;
using Models.People;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Models.People.Roles;
using System.Linq;
using Models.Events.Roles;
using BackEnd.DataBase;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Models.People.UserProperties;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services.ConfigureServices
{
    public class DBInitService : IConfigureWork
    {
        private readonly DBInitializeSettings options;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly DataBaseContext dbContext;
        private readonly ILogger<DBInitService> logger;

        public DBInitService(
            IOptions<DBInitializeSettings> options,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            DataBaseContext dbContext,
            ILogger<DBInitService> logger)
        {
            this.options = options.Value;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task Configure()
        {
            if (options.Users?.Any() == true)
                await CreateUsers();
            await CreateRoles();
            if (options.WantedRoles?.Any() == true)
                await ApplyRoles();
            await CreateUserPropertyTypes();
        }

        private async Task CreateUsers()
        {
            foreach (var user in options.Users)
            {
                var result = await userManager.CreateAsync(user, user.Password);
                logger.LogInformation(JsonConvert.SerializeObject(result));
            }
        }

        private async Task CreateRoles()
        {
            foreach (var roleName in Enum.GetValues(typeof(RoleNames)).Cast<RoleNames>())
            {
                var result = await roleManager.CreateAsync(new Role { Name = roleName.ToString() });
                logger.LogInformation(JsonConvert.SerializeObject(result));
            }
        }

        private async Task ApplyRoles()
        {
            foreach (var wantPair in options.WantedRoles)
            {
                var targetUser = await userManager.FindByEmailAsync(wantPair.Email);
                var targetRole = await roleManager.FindByNameAsync(wantPair.RoleName);
                if (targetUser == null || targetRole == null)
                    throw new Exception($"Can't find user {wantPair.Email} or role {wantPair.RoleName}");
                if (await userManager.IsInRoleAsync(targetUser, targetRole.Name)) continue;
                var result = await userManager.AddToRoleAsync(targetUser, targetRole.Name);
                logger.LogInformation(JsonConvert.SerializeObject(result));
            }
        }

        private async Task CreateUserPropertyTypes()
        {
            foreach (var typeName in Enum.GetValues(typeof(UserPropertyNames)).Cast<UserPropertyNames>())
            {
                if (await dbContext.UserPropertyTypes.AnyAsync(t => t.Name == typeName.ToString()))
                    continue;
                await dbContext.AddAsync(new UserPropertyType {
                    Name = typeName.ToString(),
                    DefaultStatus = UserPropertyStatus.NotConfirmed
                });
                await dbContext.SaveChangesAsync();
            }
        }

    }
}
