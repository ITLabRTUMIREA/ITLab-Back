using AutoMapper.Configuration;
using BackEnd.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BackEnd.Services.ConfigureServices
{
    public class ApplyMigration : IConfigureWork
    {
        private readonly DataBaseContext dbContext;
        private readonly ILogger<ApplyMigration> logger;
        private int tryCount = 10;
        private TimeSpan tryPeriod = TimeSpan.FromSeconds(10);

        public ApplyMigration(
            DataBaseContext dbContext,
            ILogger<ApplyMigration> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task Configure()
        {
            try
            {
                var pending = await dbContext.Database.GetPendingMigrationsAsync();
                if (pending?.Count() != 0)
                    await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                if (tryCount == 0)
                    throw;
                logger.LogWarning(ex, "Error while apply migrations");
                tryCount--;
                await Task.Delay(tryPeriod);
                await Configure();
            }
        }
    }
}
