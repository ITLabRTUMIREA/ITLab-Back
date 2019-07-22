using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.People.Roles;

namespace BackEnd.Models.Roles
{
    class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params RoleNames[] roles) : base()
        {
            Roles = string.Join(',', roles.Select(r => r.ToString()));
        }
    }
}
