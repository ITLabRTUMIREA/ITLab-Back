using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WebApp.Configure.Models.Shared;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Configure.Models.Configure.Interfaces;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using WebApp.Configure.Models.Shared.Interfaces;

namespace WebApp.Configure.Models.Invokations
{
    public class ConfigureExecutorHostedService : BackgroundService
    {
        private readonly ConfigureBuilder configureBuilder;
        private readonly IWorkPathGetter workPathState;
        private readonly ILogger<ConfigureExecutorHostedService> logger;
        private readonly IServiceProvider serviceProvider;

        private readonly List<IConfigurationWorkBuilder> builders;

        private List<(Task<int> task, IConfigurationWorkBuilder builder, IConfigureWork work, IServiceScope scope, int id)> work = new List<(Task<int> task, IConfigurationWorkBuilder builder, IConfigureWork work, IServiceScope scope, int id)>();

        public ConfigureExecutorHostedService(
            ConfigureBuilder configureBuilder,
            IWorkPathGetter workPathState,
            IServiceProvider serviceProvider,
            ILogger<ConfigureExecutorHostedService> logger)
        {
            this.configureBuilder = configureBuilder;
            this.workPathState = workPathState;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            builders = configureBuilder.Builders.ToList();
            workPathState.SetHandlePath(builders
                                .Select(b => b.WorkHandlePath)
                                .DefaultIfEmpty(Behavior.WorkHandlePath.Continue)
                                .Max());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastWorkId = 0;
            work = configureBuilder
                .Builders
                .Select(builder =>
                {
                    var scope = serviceProvider.CreateScope();
                    return new
                    {
                        id = Interlocked.Increment(ref lastWorkId),
                        scope,
                        builder,
                        congifurator = scope.ServiceProvider.GetService(builder.ConfigureWorkType) as IConfigureWork
                    };
                })
                .Where(b => b.congifurator != null)
                .Select(b => (b.congifurator.Configure().ContinueWith(t => b.id, stoppingToken), b.builder, b.congifurator, b.scope, b.id))
                .ToList();
            var tasks = work.Select(w => w.task).ToList();
            while (tasks.Count != 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);

                var completedBuilds = work.Where(wi => !wi.task.IsCompleted);
                workPathState.SetHandlePath(completedBuilds
                    .Select(wi => wi.builder.WorkHandlePath)
                    .DefaultIfEmpty(Behavior.WorkHandlePath.Continue)
                    .Max());
                logger.LogInformation(BuildStatus());
                var workItem = work.SingleOrDefault(w => w.id == completed.Result);
                workItem.scope?.Dispose();
            }
        }


        private string BuildStatus()
        {
            var builder = new StringBuilder();
            builder.AppendLine("CURRENT CONFIGURE BUILD STATUS: ");
            foreach (var (task, workBuilder, configureWork, _, _) in work)
            {
                builder.AppendLine(
                    $"{TaskIcon(task)} Work {configureWork.GetType().FullName} :: {workBuilder.WorkHandlePath} path");
                if (task.IsCanceled)
                    builder.AppendLine("    Work cancelled");

                if (!task.IsFaulted) continue;
                builder.AppendLine("    Work faulted");
                var exception = task.Exception ?? new Exception("Exception in task is null, what?");
                builder.AppendLine(task.Exception?.GetType().FullName);
                builder.AppendLine(task.Exception?.Message);
                if (exception is AggregateException aggregate)
                    foreach (var inner in aggregate.InnerExceptions)
                    {
                        builder.AppendLine($"       {inner.GetType().FullName}");
                        builder.AppendLine($"       {inner.Message}");
                        builder.AppendLine($"       {inner.StackTrace}");
                    }
                else
                {
                    builder.AppendLine(exception.StackTrace);
                }
            }
            return builder.ToString();
        }

        private static char TaskIcon(Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    return '-';
                case TaskStatus.Faulted:
                    return 'X';
                case TaskStatus.RanToCompletion:
                    return '+';
                default:
                    return '~';
            }
        }
    }
}
