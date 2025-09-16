using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DefaultTemplatePeriodicRunner
    /// </summary>
    public class DefaultTemplatePeriodicRunner : BackgroundService
    {
        private readonly IOptionsMonitor<DefaultTemplateUpdaterConfig> config;
        private readonly IServiceScopeFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTemplatePeriodicRunner" />
        /// class.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="factory">The factory.</param>
        public DefaultTemplatePeriodicRunner(
            IOptionsMonitor<DefaultTemplateUpdaterConfig> config,
            IServiceScopeFactory factory)
        {
            this.config = config;
            this.factory = factory;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // TODO: config onchange
            using var timer = new PeriodicTimer(config.CurrentValue.Interval);

            bool updateDefaultTemplate = config.CurrentValue.UpdateDefaultTemplate;

            while (!stoppingToken.IsCancellationRequested && updateDefaultTemplate)
            {
                await using var asyncScope = factory.CreateAsyncScope();
                var updater = asyncScope.ServiceProvider.GetRequiredService<DefaultTemplateUpdater>();
                var log = asyncScope.ServiceProvider.GetRequiredService<ILog<DefaultTemplatePeriodicRunner>>();

                try
                {
                    Guid omniClientId = Guid.NewGuid();
                    string ansidValue = string.Empty;
                    await updater.RunAsync(ansidValue, stoppingToken, omniClientId);
                }
                catch (Exception ex)
                {
                    log.Add(LogLevel.Critical, ex.Message, ex);
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
    }
}
