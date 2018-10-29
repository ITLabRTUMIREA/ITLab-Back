using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.People.Roles;

namespace BackEnd.Models.Roles
{
    class RequireRoleAttribute : TypeFilterAttribute
    {
        public RequireRoleAttribute(params RoleNames[] roles) : base(typeof(RequireRoleFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
