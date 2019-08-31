using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Models.People;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses.Login;
using Models.PublicAPI.Responses.People;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.People.Roles;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/Authentication")]
    public class AuthenticationController : AuthorizeController
    {
        private readonly IJwtFactory jwtFactory;
        private readonly IOptions<JwtIssuerOptions> jwtOptions;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public AuthenticationController(
            UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IMapper mapper,
            ILogger<AuthenticationController> logger) : base(userManager)
        {
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] AccountLoginRequest loginData)
        {
            var user = await UserManager.FindByNameAsync(loginData.Username);
            if (user == null)
                return Unauthorized();

            if (!await UserManager.CheckPasswordAsync(user, loginData.Password))
            {
                return Unauthorized();
            }
            return await GenerateResponse(user, HttpContext.Request.Headers["User-Agent"].ToString());
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Refresh([FromBody]string refreshToken)
        {
            var token = await jwtFactory.GetRefreshToken(refreshToken);
            if (token == null)
                return IncorrectRefreshToken();

            var httpUserAgent = Request.Headers["User-Agent"].ToString();

            var now = DateTime.UtcNow;
            var age = now - token.CreateTime;
            logger.LogDebug($"Token age: {age}");
            if (age > jwtOptions.Value.RefreshTokenValidFor)
                return IncorrectRefreshToken();
            return await GenerateResponse(token.User, httpUserAgent, token.Id);
        }

        [HttpGet("refresh")]
        public async Task<ActionResult<List<RefreshTokenView>>> RefreshList()
        => await jwtFactory.RefreshTokens(UserId)
                     .ProjectTo<RefreshTokenView>(mapper.ConfigurationProvider)
                     .ToListAsync();

        [HttpDelete("refresh")]
        public async Task<List<Guid>> DeleteRefreshTokens([FromBody]List<Guid> tokenIds)
        {
            await jwtFactory.DeleteRefreshTokens(tokenIds);
            return tokenIds;
        }

        private BadRequestObjectResult IncorrectRefreshToken()
            => BadRequest("Incorrect refresh token");

        private async Task<LoginResponse> GenerateResponse(User user, string userAgent, Guid? oldRefreshTokenId = null)
        {
            var roles = await UserManager.GetRolesAsync(user);
            var identity = jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString(), roles.Select(r => Enum.Parse(typeof(RoleNames), r)).Cast<RoleNames>());
            var loginInfo = new LoginResponse
            {
                User = mapper.Map<UserView>(user),
                AccessToken = jwtFactory.GenerateAccessToken(user.UserName, identity),
                RefreshToken = await jwtFactory.GenerateRefreshToken(user.Id, userAgent, oldRefreshTokenId),
                Roles = roles.ToList()
            };
            return loginInfo;
        }
    }
}