using Microsoft.AspNetCore.Identity;
using Models.DataBaseLinks;
using System;
using System.Collections.Generic;
using Models.Equipments;

namespace Models.People
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<UserSetting> UserSettings { get; set; }
        public List<Equipment> Equipment  { get; set; }
        public List<PlaceUserRole> PlaceUserRoles { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
