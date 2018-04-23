using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Authorize;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Login;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/Authentication")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IJwtFactory jwtFactory;
        private readonly IOptions<JwtIssuerOptions> jwtOptions;
        private readonly IMapper mapper;
        private readonly DataBaseContext context;

        public AuthenticationController(UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IMapper mapper,
            DataBaseContext context)
        {
            this.userManager = userManager;
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions;
            this.mapper = mapper;
            this.context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<OneObjectResponse<LoginResponse>> Login([FromBody] AccountLoginRequest loginData)
        {
            var user = await userManager.FindByNameAsync(loginData.Username);
            if (user == null)
            {
                throw ApiLogicException.Create(ResponseStatusCode.UserNotFound);
            }
            if (!await userManager.CheckPasswordAsync(user, loginData.Password))
            {
                throw ApiLogicException.Create(ResponseStatusCode.WrongPassword);
            }
            return GenerateResponse(user);
        }

        private LoginResponse GenerateResponse(User user, string token = "")
        {
            var loginInfo = mapper.Map<LoginResponse>(user);
            loginInfo.Token = token;

            var identity = jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString()/*, userManager.GetRolesAsync(user).Result.ToArray()*/);

            loginInfo.Token = Tokens.GenerateJwt(identity, jwtFactory, user.UserName).Result;
            return loginInfo;
        }
    }
}