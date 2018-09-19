using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models.People;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI;
using Microsoft.Extensions.Logging;
using Models.DataBaseLinks;
using Models.Events.Roles;
using Models.People.Roles;

namespace BackEnd.DataBase
{
    public class DataBaseFiller
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<DataBaseFiller> logger;
        private readonly DataBaseContext dbContext;
        private readonly DBInitialize options;

        public DataBaseFiller(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IOptions<DBInitialize> options,
            ILogger<DataBaseFiller> logger,
            DataBaseContext dbContext)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this.dbContext = dbContext;
            this.options = options.Value;
        }

        public async Task Fill()
        {
            foreach (var roleName in Enum.GetValues(typeof(RoleNames)).Cast<RoleNames>())
            {
                await roleManager.CreateAsync(new Role { Name = roleName.ToString() });
            }
            if (options.WantedRoles != null)
                foreach (var wantPair in options.WantedRoles)
                {
                    var targetUser = await userManager.FindByEmailAsync(wantPair.Email);
                    var targetRole = await roleManager.FindByNameAsync(wantPair.RoleName);
                    if (targetUser == null || targetRole == null)
                        throw new Exception($"Can't find user {wantPair.Email} or role {wantPair.RoleName}");
                    if (!await userManager.IsInRoleAsync(targetUser, targetRole.Name))
                    {
                        await userManager.AddToRoleAsync(targetUser, targetRole.Name);
                    }
                }
            var user = new User
            {
                UserName = "test@gmail.com",
                FirstName = "Tester",
                LastName = "Testerov",
                Email = "test@gmail.com",
                PhoneNumber = "+79161853166"
            };
            var result = await userManager.CreateAsync(user, "123456");
            logger.LogInformation($"creating default user: {result.Succeeded}");

            await MoveRoles();
        }

        private async Task MoveRoles()
        {
            //var eventRoles = new ConcurrentDictionary<string, EventRole>();
            //var placeUserEventRoles = (await dbContext
            //    .Users
            //    .SelectMany(u => u.PlaceUserRoles)
            //        .Include(pur => pur.Role)
            //    .ToListAsync())
            //    .Select(pur => new PlaceUserEventRole
            //    {
            //        UserId = pur.UserId,
            //        PlaceId = pur.PlaceId,
            //        EventRole = eventRoles.GetOrAdd(pur.Role.Name, new EventRole { Title = pur.Role.Name })
            //    })
            //    .ToList();
            //dbContext.AddRange(placeUserEventRoles);
            //await dbContext.SaveChangesAsync();
        }
    }
}
