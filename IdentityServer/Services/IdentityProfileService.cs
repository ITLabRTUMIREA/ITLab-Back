using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
        private readonly UserManager<User> _userManager;

        public IdentityProfileService(IUserClaimsPrincipalFactory<User> claimsFactory, UserManager<User> userManager)
        {
            _claimsFactory = claimsFactory;
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            if (user == null)
            {
                throw new ArgumentException($"Can not find user by id {sub}");
            }
            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();
            AddClaim(claims, "first_name", user.FirstName);
            AddClaim(claims, "last_name", user.LastName);
            AddClaim(claims, "middle_name", user.MiddleName);
            context.IssuedClaims = claims;
        }

        private void AddClaim(List<Claim> claims, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                claims.Add(new Claim(key, value));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }

    }
}
