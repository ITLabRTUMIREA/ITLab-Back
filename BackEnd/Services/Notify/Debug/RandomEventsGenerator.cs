using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models.PublicAPI.NotifyRequests;

namespace BackEnd.Services.Notify.Debug
{
    /// <summary>
    /// Generate test event with int field for testing
    /// </summary>
    public class RandomEventsGenerator : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        /// <summary>
        /// Require service provider for receiving
        /// </summary>
        /// <param name="serviceProvider"></param>
        public RandomEventsGenerator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Generate event with int every 4 seconds
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventId = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(4), stoppingToken);
                serviceProvider.GetRequiredService<INotifier>().Notify(NotifyType.EventChange, new { id = eventId });
                eventId++;
            }
        }
    }
}
