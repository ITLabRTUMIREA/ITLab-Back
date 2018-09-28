using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WebApp.Configure.Models.Shared;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Configure.Models.Configure.Interfaces;
using System.Collections.Generic;
using WebApp.Configure.Models.Shared.Interfaces;

namespace WebApp.Configure.Models.Invokations
{
    public class ConfigureExecutorHostedService : BackgroundService
    {
        private readonly ConfigureBuilder configureBuilder;
        private readonly IWorkPathGetter workPathState;
        private readonly IServiceProvider serviceProvider;

        private readonly List<IConfigurationWorkBuilder> builders;

        public ConfigureExecutorHostedService(
            ConfigureBuilder configureBuilder,
            IWorkPathGetter workPathState,
            IServiceProvider serviceProvider)
        {
            this.configureBuilder = configureBuilder;
            this.workPathState = workPathState;
            this.serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            builders = configureBuilder.Builders.ToList();
            workPathState.SetHandlePath(builders
                                .Select(b => b.WorkHandlePath)
                                .DefaultIfEmpty(Behavior.WorkHandlePath.Continue)
                                .Max());
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = configureBuilder
                .Builders
                .Select(b => new { builder = b, congifurator = serviceProvider.GetService(b.ConfigureWorkType) as IConfigureWork })
                .Where(b => b.congifurator != null)
                .Select(b => b.congifurator.Configure().ContinueWith((task) => builders.Remove(b.builder)))
                .ToList();
            while (tasks.Count != 0)
            {
                await Task.WhenAny(tasks);
                tasks.RemoveAll(t => t.IsCanceled || t.IsCompleted || t.IsFaulted);
                workPathState.SetHandlePath(builders
                    .Select(b => b.WorkHandlePath)
                    .DefaultIfEmpty(Behavior.WorkHandlePath.Continue)
                    .Max());
            }
        }
    }
}
