using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Configure.Models.Configure.Interfaces;

namespace IdentityServer.Services.Configure
{
    public class FillConfigurationConfigureWork : IConfigureWork
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<FillConfigurationConfigureWork> logger;
        private readonly ConfigurationDbContext db;

        public FillConfigurationConfigureWork(
            IConfiguration configuration,
            ILogger<FillConfigurationConfigureWork> logger,
            ConfigurationDbContext db)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.db = db;
        }
        public async Task Configure()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    await InternalConfigure().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "error while fiil db, wait 5 seconds");
                    await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    throw;
                }
            }
        }

        private async Task InternalConfigure()
        {
            await IdentityResources().ConfigureAwait(false);
            await Clients().ConfigureAwait(false);
            await Apis().ConfigureAwait(false);
        }

        private async Task IdentityResources()
        {
            logger.LogInformation("Checking identity resources");
            foreach (var identityResource in Config.GetIdentityResources())
            {
                logger.LogInformation($"Checking {identityResource.Name}");
                if (!db.IdentityResources.Any(ir => ir.Name == identityResource.Name))
                {
                    logger.LogInformation($"No {identityResource.Name} recource, apllying");
                    db.IdentityResources.Add(identityResource.ToEntity());
                    var savedCount = await db.SaveChangesAsync().ConfigureAwait(false);
                    if (savedCount == 0)
                    {
                        logger.LogWarning("No saved entity while adding identity resource");
                    }
                }
            }
        }
        private async Task Clients()
        {
            logger.LogInformation("Checking clients");
            var clients = new List<Client>();
            ConfigurationBinder.Bind(configuration.GetSection("Clients"), clients);
            foreach (var client in clients)
            {
                logger.LogInformation($"Checking {client.ClientName}");
                if (!db.Clients.Any(ir => ir.ClientName == client.ClientName))
                {
                    logger.LogInformation($"No {client.ClientName} client, apllying");
                    db.Clients.Add(client.ToEntity());
                    var savedCount = await db.SaveChangesAsync().ConfigureAwait(false);
                    if (savedCount == 0)
                    {
                        logger.LogWarning("No saved entity while adding client");
                    }
                }
            }
        }

        private async Task Apis()
        {
            logger.LogInformation("Checking apis");
            foreach (var api in Config.GetApis())
            {
                logger.LogInformation($"Checking {api.Name}");
                if (!db.ApiResources.Any(a => a.Name== api.Name))
                {
                    logger.LogInformation($"No {api.Name} api, apllying");
                    db.ApiResources.Add(api.ToEntity());
                    var savedCount = await db.SaveChangesAsync().ConfigureAwait(false);
                    if (savedCount == 0)
                    {
                        logger.LogWarning("No saved entity while adding client");
                    }
                }
            }
        }
    }
}
