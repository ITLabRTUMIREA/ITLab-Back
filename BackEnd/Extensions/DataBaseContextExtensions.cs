using BackEnd.DataBase;
using Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.PublicAPI;

namespace BackEnd.Extensions
{
    public static class DataBaseContextExtensions
    {
        public static void FillDefaults(this DataBaseContext context, DBInitialize data)
        {
            foreach (var role in RoleNames.List)
            {
                context.Roles.Add(new Role { Name = role });
            }
            context.SaveChanges();
        }
    }
}
