using Microsoft.AspNetCore.Identity;
using Models.DataBaseLinks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.People
{
    public class Role : IdentityRole<Guid>
    {
        public List<PlaceUserRole> PlaceUserRoles { get; set; }
    }
}
