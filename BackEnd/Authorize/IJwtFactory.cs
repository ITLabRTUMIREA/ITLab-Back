using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Models.People;
using Models.People.Roles;

namespace BackEnd.Authorize
{
    public interface IJwtFactory
    {
        string GenerateAccessToken(string userName, ClaimsIdentity identity);
        Task<string> GenerateRefreshToken(Guid userId, string userAgent, Guid? refreshTokenId = default);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        IQueryable<RefreshToken> RefreshTokens(Guid userId);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id, IEnumerable<RoleNames> roles);
        Task DeleteRefreshTokens(List<Guid> tokenIds);
    }
}
