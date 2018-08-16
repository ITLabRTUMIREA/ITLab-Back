using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.PublicAPI;

namespace BackEnd.DataBase
{
    public class DataBaseFiller
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly DBInitialize options;

        public DataBaseFiller(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IOptions<DBInitialize> options)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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
        }
    }
}
