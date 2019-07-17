using BackEnd.DataBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Configure.Models.Configure.Interfaces;

namespace IdentityServer.Services.Configure
{
    public class DefaultUserConfigureWork : IConfigureWork
    {
        public const string UserEmail = "test@test.com";
        public const string Password = "123456Az*";
        private readonly UserManager<User> userManager;
        private readonly ILogger<DefaultUserConfigureWork> logger;

        public DefaultUserConfigureWork(UserManager<User> userManager, ILogger<DefaultUserConfigureWork> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task Configure()
        {
            var user = await userManager.FindByEmailAsync(UserEmail);
            if (user != null)
            {
                logger.LogInformation($"User {UserEmail} already exists");
                return;
            }
            logger.LogInformation($"No user with email {UserEmail}");
            var result = userManager.CreateAsync(new User
            {
                Email = UserEmail,
                UserName = UserEmail
            }, Password);
            logger.LogInformation($"User created : {result.IsCompletedSuccessfully}");
        }
    }
}
