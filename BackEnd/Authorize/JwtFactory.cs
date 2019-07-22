using Microsoft.Extensions.Options;
using Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BackEnd.DataBase;
using Models.People;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Models.People.Roles;

namespace BackEnd.Authorize
{
    public partial class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly ILogger<JwtFactory> logger;
        private readonly DataBaseContext dbContext;
        private const string RefreshTokenChars = "ABCDEFGHIKLMNOPQRSTVXYZ";
        private readonly Random random = new Random();

        public JwtFactory(
            IOptions<JwtIssuerOptions> jwtOptions,
            ILogger<JwtFactory> logger,
            DataBaseContext dbContext)
        {
            this.jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(this.jwtOptions);
            this.logger = logger;
            this.dbContext = dbContext;
        }
        public string GenerateAccessToken(string userName, ClaimsIdentity identity)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
            }.Concat(identity.Claims).ToArray();

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: jwtOptions.NotBefore,
                expires: jwtOptions.Expiration,
                signingCredentials: jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
        public ClaimsIdentity GenerateClaimsIdentity(string userName, string id, IEnumerable<RoleNames> roles)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim(ClaimTypes.Role, RolesHelper.RoleString(roles)));
            return identity;
        }
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        public async Task<string> GenerateRefreshToken(Guid userId, string userAgent, Guid? refreshTokenId = default)
        {
            var token = new RefreshToken
            {
                CreateTime = DateTime.UtcNow,
                UserId = userId,
                UserAgent = userAgent,
                Token = RandomString(50)
            };
            var targetRow = await dbContext
                .RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Id == refreshTokenId && rt.UserId == userId);

            if (targetRow != null)
            {
                targetRow.UserAgent = userAgent;
                targetRow.CreateTime = token.CreateTime;
                targetRow.Token = token.Token;
            }
            else
            {
                targetRow = token;
                dbContext.RefreshTokens.Add(token);
            }
            await dbContext.SaveChangesAsync();
            return ToBase64(new RefreshTokenData
            {
                RefreshTokenId = targetRow.Id,
                UserId = userId,
                Token = targetRow.Token,
                UserAgent = userAgent
            });
        }

        public IQueryable<RefreshToken> RefreshTokens(Guid userId)
            => dbContext.RefreshTokens.Where(rt => rt.UserId == userId);


        public Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var tokenData = FromBase64<RefreshTokenData>(refreshToken);
            return dbContext.RefreshTokens
                                 .Where(rt => rt.Id == tokenData.RefreshTokenId)
                                 .Where(rt => rt.UserId == tokenData.UserId)
                                 .Where(rt => rt.Token == tokenData.Token)
                                 .Include(rt => rt.User)
                                 .SingleOrDefaultAsync();
        }


        public async Task DeleteRefreshTokens(List<Guid> tokenIds)
        {
            var targetTokens = await dbContext
                .RefreshTokens
                .Where(rt => tokenIds.Contains(rt.Id))
                .ToListAsync();
            dbContext.RemoveRange(targetTokens);
            await dbContext.SaveChangesAsync();
        }

        private string RandomString(int length)
            => new string(Enumerable.Repeat(RefreshTokenChars, length)
                     .Select(chars => chars[random.Next(chars.Length)])
                          .ToArray());

        private static string ToBase64<T>(T refreshTokenData)
        {
            var jsonView = JsonConvert.SerializeObject(refreshTokenData);
            var bytes = Encoding.UTF8.GetBytes(jsonView);
            return Convert.ToBase64String(bytes);
        }
        private static T FromBase64<T>(string refreshTokenData)
        {
            var bytes = Convert.FromBase64String(refreshTokenData);
            var jsonView = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(jsonView);
        }

        
    }
}

