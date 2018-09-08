﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.PublicAPI;
using Microsoft.Extensions.Logging;

namespace BackEnd.DataBase
{
    public class DataBaseFiller
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<DataBaseFiller> logger;
        private readonly DBInitialize options;

        public DataBaseFiller(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IOptions<DBInitialize> options,
            ILogger<DataBaseFiller> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task Fill()
        {
            foreach (var roleName in RoleNames.List)
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            if (options.WantedRoles != null)
                foreach (var wantPair in options.WantedRoles)
                {
                    var targetUser = await userManager.FindByEmailAsync(wantPair.Email);
                    var targetRole = await roleManager.FindByNameAsync(wantPair.RoleName);
                    if (targetUser == null || targetRole == null)
                        throw new Exception($"Can't fint user {wantPair.Email} or role {wantPair.RoleName}");
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
        }
    }
}
