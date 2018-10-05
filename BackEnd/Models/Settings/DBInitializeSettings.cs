using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.People;

namespace BackEnd.Models.Settings
{
    public class DBInitializeSettings
    {
        public List<UserAndPassword> Users { get; set; }
        public List<UserToRole> WantedRoles { get; set; }
    }

    public class UserAndPassword : User
    {
        public string Password { get; set; }
    }

    public class UserToRole
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
}
