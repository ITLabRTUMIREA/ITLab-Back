using Microsoft.AspNetCore.Identity;
using Models.DataBaseLinks;
using System;
using System.Collections.Generic;
using Models.Equipments;
using Models.People.UserProperties;

namespace Models.People
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public List<UserSetting> UserSettings { get; set; }
        public List<Equipment> Equipment  { get; set; }
        public List<EquipmentOwnerChangeRecord> EquipmentOwnerChangeRecords { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public List<PlaceUserEventRole> PlaceUserEventRoles { get; set; }
        public List<UserProperty> UserProperties { get; set; }
    }
}
