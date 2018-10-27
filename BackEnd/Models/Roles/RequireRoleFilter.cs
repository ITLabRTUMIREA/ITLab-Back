using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.People.Roles;

namespace BackEnd.Models.Roles
{
    public class RequireRoleFilter : IAuthorizationFilter
    {
        private readonly RoleNames[] needRoles;

        public RequireRoleFilter(params RoleNames[] needRoles)
        {
            this.needRoles = needRoles;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var targetClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role);
            if (targetClaim == null)
                return;
            var roles = GetNames(targetClaim.Value);
            if (roles.Intersect(needRoles).Count() != needRoles.Length)
                context.Result = new ForbidResult();
        }

        private static IEnumerable<RoleNames> GetNames(string encoded)
        {
            var buffer = Convert.FromBase64String(encoded);
            for (var i = 0; i < buffer.Length; i++)
            for (var j = 0; j < Powers.Length; j++)
                if ((buffer[i] & Powers[j]) != 0)
                    yield return (RoleNames)(i * 8 + j);
        }
        private static readonly byte[] Powers = Enumerable
            .Range(0, 8)
            .Select(v => Math.Pow(2, v))
            .Select(v => (byte)v)
            .ToArray();
    }
}
