using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Configure.Models.Configure.Interfaces;

namespace IdentityServer.Services.Configure
{
    public class MigrateConfigureDatabaseWork : IConfigureWork
    {
        private readonly ILogger<MigrateConfigureDatabaseWork> logger;
        private readonly ConfigurationDbContext db;

        public MigrateConfigureDatabaseWork(ILogger<MigrateConfigureDatabaseWork> logger, ConfigurationDbContext db)
        {
            this.logger = logger;
            this.db = db;
        }
        public async Task Configure()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var pending = await db.Database.GetPendingMigrationsAsync().ConfigureAwait(false);
                    if (pending?.Count() != 0)
                        await db.Database.MigrateAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "error while apply migrations");
                }
            }
        }
    }
}
