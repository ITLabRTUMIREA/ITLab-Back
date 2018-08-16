using System;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BackEnd.DataBase;
using System.ComponentModel;
using Extensions;
using System.Linq;
using System.Reflection;
using Models.PublicAPI;
namespace BackEnd.Services
{
    public class RoleAccessor : IRolesAccessor
    {
        [Description(RoleNames.ParticipantRoleName)]
        public Guid ParticipantRoleId { get; set; }

        [Description(RoleNames.OrginizerRoleName)]
        public Guid OrginizerRoleId { get; set; }

        public RoleAccessor(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<DataBaseContext>();
                var roles = dbContext.Roles.ToList();
                GetType()
                    .GetProperties()
                    .DoForEach(p => p.SetValue(this, roles.Single(r => r.Name == p.GetCustomAttribute<DescriptionAttribute>().Description).Id));
            }
        }

    }
}
