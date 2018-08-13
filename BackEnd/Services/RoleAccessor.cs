using System;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BackEnd.DataBase;
using System.ComponentModel;
using Extensions;
using System.Linq;
using System.Reflection;
namespace BackEnd.Services
{
    public class RoleAccessor : IRolesAccessor
    {
        [Description("Participant")]
        public Guid ParticipantRoleId { get; set; }
        [Description("Orginizer")]
        public Guid OrginizerRoleId { get; set; }
        [Description("Invited")]
        public Guid InvitedRoleId { get; set; }
        [Description("Wishing")]
        public Guid WishingRoleId { get; set; }

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
