using BackEnd.DataBase;
using Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Extensions
{
    public static class DataBaseContextExtensions
    {
        public static void FillDefaults(this DataBaseContext context, DBInitialize data)
        {
            foreach (var role in data.NeededStandartRoles)
            {
                context.Roles.Add(new Role { Name = role });
            }
            context.SaveChanges();
        }
    }
}
