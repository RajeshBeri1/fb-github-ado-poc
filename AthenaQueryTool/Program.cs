using Amazon.Athena;
using AutoMapper;
using Lamar;
using Lib.Athena.Business;
using Lib.Athena.Business.Interfaces;
using Lib.Athena.Models;
using Lib.Aurora.Business;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.SampleData.Business;
using Lib.WebAPI.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AthenaQueryTool
{
    internal static class Program
    {
        [STAThread]
        private static async Task Main()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var useSampleData = configuration.GetValue<bool>("UseSampleData");

            using var container = await Container.BuildAsync(registry =>
            {
                registry.Configure<AWSCredentialConfig>(configuration.GetSection(nameof(AWSCredentialConfig)));
                registry.Configure<AthenaQueryConfig>(configuration.GetSection(nameof(AthenaQueryConfig)));

                registry.For<ICorrelationIdGenerator>().Use<CorrelationIdGenerator>().Scoped();

                registry.AddLogging();

                registry.For(typeof(ILog<>)).Use(typeof(Log<>));

                registry.For<IMapper>().Use(AutomapperConfiguration.GetMapper()).Singleton();

                if (useSampleData)
                {
                    registry.AddSingleton<SampleDataDbContext>();
                    registry.AddTransient<IAuroraQueryLogic, AthenaSampleDataQueryLogic>();
                }
                else
                {
                    registry.For<IAuroraQueryLogic>().Use<AuroraQueryLogic>();

                    registry.AddDefaultAWSOptions(provider =>
                    {
                        var credentialConfig = provider.GetRequiredService<IOptionsMonitor<AWSCredentialConfig>>();
                        var queryConfig = provider.GetRequiredService<IOptionsMonitor<AthenaQueryConfig>>();
                        var options = configuration.GetAWSOptions();
                        var log = provider.GetRequiredService<ILog<RoleBasedAWSCredentials>>();

                        options.Credentials = new RoleBasedAWSCredentials(credentialConfig, options, configuration, queryConfig, log);

                        return options;
                    });

                    registry.AddAWSService<IAmazonAthena>();
                }
            });

            if (useSampleData)
            {
                using var source = new CancellationTokenSource();
                var sampleDb = container.GetInstance<SampleDataDbContext>();
                await sampleDb.EnsureCreatedAsync(source.Token);
            }

            ApplicationConfiguration.Initialize();
            Application.Run(container.GetInstance<MainWindow>());
        }
    }
}