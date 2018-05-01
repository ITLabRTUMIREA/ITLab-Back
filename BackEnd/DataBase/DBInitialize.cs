using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DataBase
{
    public class DBInitialize
    {
        public List<string> NeededStandartRoles { get; set; }
        public List<UserToRole> WantedRoles { get; set; }
    }
    
    public class UserToRole
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
}
