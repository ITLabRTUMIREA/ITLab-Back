using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Models.People;

namespace BackEnd.Authorize
{
    public interface IJwtFactory
    {
        string GenerateAccessToken(string userName, ClaimsIdentity identity);
        Task<string> GenerateRefreshToken(Guid userId, string UserAgent);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        IQueryable<RefreshToken> RefreshTokens(Guid userId);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id/*string[] roles*/);
        Task DeleteRefreshTokens(List<Guid> tokenIds);
    }
}
