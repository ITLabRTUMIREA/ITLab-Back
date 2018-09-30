using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Models.People.Roles;

namespace BackEnd.Models.Roles
{
    class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params RoleNames[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
