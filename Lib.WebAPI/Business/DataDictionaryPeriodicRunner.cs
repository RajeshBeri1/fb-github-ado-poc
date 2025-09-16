using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DataDictionaryPeriodicRunner
    /// </summary>
    public class DataDictionaryPeriodicRunner : BackgroundService
    {
        private readonly IDbContextFactory<PortalDbContext> contextFactory;
        private readonly IOptionsMonitor<DataDictionaryUpdaterConfig> config;
        private readonly IServiceScopeFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDictionaryPeriodicRunner" />
        /// class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="factory">The factory.</param>
        public DataDictionaryPeriodicRunner(
            IDbContextFactory<PortalDbContext> contextFactory,
            IOptionsMonitor<DataDictionaryUpdaterConfig> config,
            IServiceScopeFactory factory)
        {
            this.contextFactory = contextFactory;
            this.config = config;
            this.factory = factory;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // TODO: config onchange
            using var timer = new PeriodicTimer(config.CurrentValue.Interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var context = await contextFactory.CreateDbContextAsync())
                {
                    await using var asyncScope = factory.CreateAsyncScope();
                    var updater = asyncScope.ServiceProvider.GetRequiredService<DataDictionaryUpdater>();
                    var log = asyncScope.ServiceProvider.GetRequiredService<ILog<DataDictionaryPeriodicRunner>>();

                    try
                    {
                        await updater.RunAsync(stoppingToken);
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
}