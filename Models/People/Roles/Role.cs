using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Models.DataBaseLinks;

namespace Models.People.Roles
{
    public class Role : IdentityRole<Guid>
    {
        public List<PlaceUserRole> PlaceUserRoles { get; set; }
    }
}
