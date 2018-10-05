using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Authorize;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Models.People;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Authentication.Twitter;
using Models.PublicAPI.Responses.People;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/Authentication")]
    public class AuthenticationController : AuthorizeController
    {
        private readonly IJwtFactory jwtFactory;
        private readonly IOptions<JwtIssuerOptions> jwtOptions;
        private readonly IMapper mapper;

        public AuthenticationController(
            UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IMapper mapper) :base (userManager)
        {
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions;
            this.mapper = mapper;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<OneObjectResponse<LoginResponse>> Login([FromBody] AccountLoginRequest loginData)
        {
            var user = await userManager.FindByNameAsync(loginData.Username) ??
                throw ApiLogicException.Create(ResponseStatusCode.WrongLoginOrPassword);

            if (!await userManager.CheckPasswordAsync(user, loginData.Password))
            {
                throw ApiLogicException.Create(ResponseStatusCode.WrongLoginOrPassword);
            }
            return await GenerateResponse(user, HttpContext.Request.Headers["User-Agent"].ToString());
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<OneObjectResponse<LoginResponse>> Refresh([FromBody]string refreshToken)
        {
            var token = await jwtFactory.GetRefreshToken(refreshToken)
                                        ?? throw IncorrectRefreshToken();
            if (DateTime.UtcNow - token.CreateTime > jwtOptions.Value.RefreshTokenValidFor
                || Request.Headers["User-Agent"].ToString() != token.UserAgent)
                throw IncorrectRefreshToken();
            return await GenerateResponse(token.User, token.UserAgent);
        }

        [HttpGet("refresh")]
        public async Task<ListResponse<RefreshTokenView>> RefreshList()
        => await jwtFactory.RefreshTokens(UserId)
                     .ProjectTo<RefreshTokenView>()
                     .ToListAsync();

        [HttpDelete("refresh")]
        public async Task<ResponseBase> DeleteRefreshTokens([FromBody]List<Guid> tokenIds)
        {
            await jwtFactory.DeleteRefreshTokens(tokenIds);
            return ResponseStatusCode.OK;
        }

        private Exception IncorrectRefreshToken()
        => ResponseStatusCode.IncorrectRefreshToken.ToApiException();

        private async Task<LoginResponse> GenerateResponse(User user, string userAgent)
        {
            var identity = jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString(), await userManager.GetRolesAsync(user));
            var loginInfo = new LoginResponse
            {
                User = mapper.Map<UserView>(user),
                AccessToken = jwtFactory.GenerateAccessToken(user.UserName, identity),
                RefreshToken = await jwtFactory.GenerateRefreshToken(user.Id, userAgent)
            };
            return loginInfo;
        }
    }
}