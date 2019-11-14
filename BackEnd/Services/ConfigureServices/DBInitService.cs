using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BackEnd.Models.Settings;
using Models.People;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Models.People.Roles;
using System.Linq;
using BackEnd.DataBase;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System.Threading;
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

        public async Task Configure(CancellationToken cancellationToken)
        {
            if (options.Users?.Any() == true)
                await CreateUsers();
            await CreateUserPropertyNames();
            await CreateRoles();
            if (options.WantedRoles?.Any() == true)
                await ApplyRoles();
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

        private async Task CreateUserPropertyNames()
        {
            foreach (var userPropertyName in Enum.GetValues(typeof(UserPropertyNames)).Cast<UserPropertyNames>())
            {
                var internalName = userPropertyName.ToString();
                var existing = dbContext.UserPropertyTypes.FirstOrDefaultAsync(upt => upt.InternalName == internalName);
                if (existing == null)
                {
                    var newType = new UserPropertyType
                    {
                        DefaultStatus = UserPropertyStatus.NotConfirmed,
                        InternalName = internalName,
                        PublicName = internalName,
                    };
                    dbContext.UserPropertyTypes.Add(newType);
                    var saved = await dbContext.SaveChangesAsync();
                    logger.LogInformation($"Added user property type {internalName}, saved: {saved}");
                }
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
    }
}
