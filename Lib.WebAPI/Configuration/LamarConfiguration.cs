using AutoMapper;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Honeycomb.OpenTelemetry;
using Lamar;
using Lib.Annalect.Business;
using Lib.Aurora.Business;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.Common.Models;
using Lib.MediaopsToFlowChart.Business;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.Consts;
using Lib.WebAPI.Extensions;
using Lib.WebAPI.Models;
using Lib.WebAPI.ThirdParty;
using Lib.WebAPI.Validation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NSubstitute;
using Okta.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;

namespace Lib.WebAPI.Configuration
{
    /// <summary>
    /// LamarConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class LamarConfiguration
    {
        /// <summary>
        /// Configures the specified registry.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        public static void Configure(ServiceRegistry registry, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            // read sensitive data from override file
            var builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(environment.ContentRootPath, "appsettings.override.json"));
            var config = builder.Build();
            registry.Configure<ConnectionStrings>(config.GetSection(nameof(ConnectionStrings)));

            registry.Configure<TaskpaneConfig>(configuration.GetSection(nameof(TaskpaneConfig)));
            registry.Configure<DataDictionaryUpdaterConfig>(configuration.GetSection(nameof(DataDictionaryUpdaterConfig)));
            registry.Configure<DefaultTemplateUpdaterConfig>(configuration.GetSection(nameof(DefaultTemplateUpdaterConfig)));
            registry.Configure<ColumnTableMappingConfig>(configuration.GetSection(nameof(ColumnTableMappingConfig)));

            registry.Configure<StorageConfig>(configuration.GetSection(nameof(StorageConfig)));

            var oktaConfig = new OktaConfig();
            configuration.GetSection(nameof(OktaConfig)).Bind(oktaConfig);
            registry.For<OktaConfig>().Use(oktaConfig).Singleton();

            var omniConfig = new OmniAuthConfig();
            configuration.GetSection(nameof(OmniAuthConfig)).Bind(omniConfig);
            registry.For<OmniAuthConfig>().Use(omniConfig).Singleton();

            var mediaOpsConfig = new MediaOpsConfig();
            configuration.GetSection(nameof(MediaOpsConfig)).Bind(mediaOpsConfig);
            registry.For<MediaOpsConfig>().Use(mediaOpsConfig).Singleton();

            var fusionConfig = new FusionCacheConfig();
            configuration.GetSection(nameof(FusionCacheConfig)).Bind(fusionConfig);
            registry.For<FusionCacheConfig>().Use(fusionConfig).Singleton();

            registry.For<IMapper>().Use(AutomapperConfiguration.GetMapper()).Singleton();

            registry.AddTransient(provider =>
            {
                var connectionStrings = provider.GetRequiredService<IOptionsMonitor<ConnectionStrings>>();
                return new BlobServiceClient(connectionStrings.CurrentValue.AzuriteBlob);
            });

            registry.AddDbContextFactory<PortalDbContext>((provider, sqlServerOptions) =>
            {
                var connectionStrings = provider.GetRequiredService<IOptionsMonitor<ConnectionStrings>>();
                sqlServerOptions.UseSqlServer(connectionStrings.CurrentValue.PortalDb, sqlOptions =>
                {
                    sqlOptions.CommandTimeout((int)TimeSpan.FromHours(1).TotalSeconds);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); // Enable retry on failure
                });
                sqlServerOptions.ReplaceUpsertCommandRunner<SqlServerUpsertCommandRunner>();
            });

            registry.AddDbContext<IPortalDbContext, PortalDbContext>((provider, sqlServerOptions) =>
            {
                var connectionStrings = provider.GetRequiredService<IOptionsMonitor<ConnectionStrings>>();
                sqlServerOptions.UseSqlServer(connectionStrings.CurrentValue.PortalDb, sqlOptions =>
                {
                    sqlOptions.CommandTimeout((int)TimeSpan.FromHours(1).TotalSeconds);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); // Enable retry on failure
                });
                sqlServerOptions.ReplaceUpsertCommandRunner<SqlServerUpsertCommandRunner>();
            });

            registry.For<IPortalUnitOfWork>().Use<PortalUnitOfWork>();
            registry.For<ICacheLogic>().Use<CacheLogic>();
            registry.For<IClientControllerLogic>().Use<ClientControllerLogic>();
            registry.For<IUserProvider>().Use<UserProvider>();
            registry.For<ICalendarTemplateControllerLogic>().Use<CalendarTemplateControllerLogic>();
            registry.For<ICalendarOverlayTemplateControllerLogic>().Use<CalendarOverlayTemplateControllerLogic>();
            registry.For<IMediaHierarchyTemplateControllerLogic>().Use<MediaHierarchyTemplateControllerLogic>();
            registry.For<IFlowchartTemplateControllerLogic>().Use<FlowchartTemplateControllerLogic>();
            registry.For<IThemeTemplateControllerLogic>().Use<ThemeTemplateControllerLogic>();
            registry.For<ITotalsTemplateControllerLogic>().Use<TotalsTemplateControllerLogic>();
            registry.For<IFlowchartTemplatesHistortyControllerLogic>().Use<FlowchartTemplatesHistortyControllerLogic>();
            registry.For<IHeaderTemplateControllerLogic>().Use<HeaderTemplateControllerLogic>();
            registry.For<IFooterTemplateControllerLogic>().Use<FooterTemplateControllerLogic>();
            registry.For<ICommonServices>().Use<CommonServices>();
            registry.For<IFieldInfoServices>().Use<FieldInfoServices>();
            registry.For<IMigrationControllerLogic>().Use<MigrationControllerLogic>();
            registry.For<IDefaultTemplateServices>().Use<DefaultTemplateServices>();
            // registry.For<IAthenaQueryLogic>().Use<AthenaQueryLogic>();
            registry.For<IAuroraQueryLogic>().Use<AuroraQueryLogic>();
            registry.For<ICorrelationIdGenerator>().Use<CorrelationIdGenerator>().Scoped();
            registry.For<ITelemetryInitializer>().Use<TrackingInitializer>().Singleton();

            registry.For(typeof(ILog<>)).Use(typeof(Log<>));

            registry.For<DataDictionaryPeriodicRunner>().Use<DataDictionaryPeriodicRunner>().Singleton();
            registry.AddHostedService(provider => provider.GetRequiredService<DataDictionaryPeriodicRunner>());

            registry.For<DefaultTemplatePeriodicRunner>().Use<DefaultTemplatePeriodicRunner>().Singleton();
            registry.AddHostedService(provider => provider.GetRequiredService<DefaultTemplatePeriodicRunner>());

            registry.AddSwaggerGenNewtonsoftSupport();
            registry.AddSignalR();
            //registry.AddDefaultAWSOptions(provider =>
            //{
            //    var credentialConfig = provider.GetRequiredService<IOptionsMonitor<AWSCredentialConfig>>();
            //    var queryConfig = provider.GetRequiredService<IOptionsMonitor<AthenaQueryConfig>>();
            //    var options = configuration.GetAWSOptions();
            //    var log = provider.GetRequiredService<ILog<RoleBasedAWSCredentials>>();
            //    var config = provider.GetRequiredService<IConfiguration>() as IConfigurationRoot;

            //    config.ThrowIfNull();

            //    options.Credentials = new RoleBasedAWSCredentials(credentialConfig, options, config, queryConfig, log);

            //    return options;
            //});

            //registry.AddAWSService<IAmazonAthena>();

            registry.AddControllers(config => config.OutputFormatters.RemoveType<StringOutputFormatter>())
                .AddControllersAsServices()
                .AddNewtonsoftJson(setup => JsonConfiguration.Configure(setup.SerializerSettings));

            registry.AddEndpointsApiExplorer();
            registry.AddSwaggerGen(setup => SwaggerConfiguration.Configure(setup, omniConfig, oktaConfig));

            registry.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) => context.HttpContext.RequestServices.ResolveExceptionHandler().Handle(context);
            });

            var authBuilder = registry.AddAuthentication(options =>
            {
                if (omniConfig.Enabled && !oktaConfig.Enabled)
                {
                    options.DefaultAuthenticateScheme = OmniAuthenticationHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = OmniAuthenticationHandler.AuthenticationScheme;
                    options.DefaultSignInScheme = OmniAuthenticationHandler.AuthenticationScheme;
                }
                else
                {
                    options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                    options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                    options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
                }
            });

            if (oktaConfig.Enabled)
            {
                authBuilder.AddOktaWebApi(new OktaWebApiOptions()
                {
                    OktaDomain = oktaConfig.Domain,
                });
            }
            else if (omniConfig.Enabled)
            {
                authBuilder.AddScheme<OmniAuthenticationSchemeOptions, OmniAuthenticationHandler>(OmniAuthenticationHandler.AuthenticationScheme, options => { });

                registry.AddHttpClient(AnnalectApiClient.ClientName, (provider, client) =>
                {
                    client.BaseAddress = new Uri($"https://{omniConfig.Domain}");
                })
                    .AddPolicyHandler((provider, request) => HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        Backoff.DecorrelatedJitterBackoffV2(omniConfig.RetryDelay, omniConfig.RetryCount),
                        onRetry: (result, timeSpan) =>
                        {
                            var log = provider.GetRequiredService<ILog<HttpClient>>();
                            log.Add(Microsoft.Extensions.Logging.LogLevel.Warning, $"Retrying Annalect HTTP client connection, retry in: {timeSpan}, reason: {result.Exception?.Message}");
                        }));

                // write code for MediaopsToFlowChartImportApi here
                registry.AddHttpClient(MediaopsToFlowChartImportApi.ClientName, (provider, client) =>
                {
                    client.BaseAddress = new Uri($"https://{mediaOpsConfig.Domain}/{mediaOpsConfig.Environment}/");
                })
                    .AddPolicyHandler((provider, request) => HttpPolicyExtensions.HandleTransientHttpError()
                                       .WaitAndRetryAsync(
                                               Backoff.DecorrelatedJitterBackoffV2(mediaOpsConfig.RetryDelay, mediaOpsConfig.RetryCount),
                                               onRetry: (result, timeSpan) =>
                                               {
                                                   var log = provider.GetRequiredService<ILog<HttpClient>>();
                                                   log.Add(Microsoft.Extensions.Logging.LogLevel.Warning, $"Retrying MediaopsToFlowChartImportApi HTTP client connection, retry in: {timeSpan}, reason: {result.Exception?.Message}");
                                               }));
            }
            else
            {
                authBuilder.AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(WebApiConsts.JWTCommunicationKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
            }

            registry.AddAuthorization();

            // using honeycomb with opentelemetry
            var options = new ApplicationInsightsServiceOptions { ConnectionString = config.GetSection("ApplicationInsights").GetRequiredSection("ConnectionString").Value };
            registry.AddApplicationInsightsTelemetry(options: options);

            registry.AddResponseCompression(x =>
            {
                x.EnableForHttps = true;
                //x.Providers.Add<ZstdCompressionProvider>();
                x.Providers.Add<BrotliCompressionProvider>();
                x.Providers.Add<GzipCompressionProvider>();
            });

            registry.Configure<BrotliCompressionProviderOptions>(x => x.Level = CompressionLevel.Fastest);
            registry.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Fastest);
            //registry.Configure<ZstdCompressionProviderOptions>(x => x.Level = -1);

            registry.AddValidatorsFromAssemblyContaining<FlowchartTemplateCreateValidator>()
                .AddFluentValidationAutoValidation()
                .AddFluentValidationRulesToSwagger();

            ValidatorOptions.Global.LanguageManager.Enabled = false;

            if (fusionConfig.Enabled)
            {
                var redisConfig = configuration.GetConnectionString(nameof(ConnectionStrings.Redis));

                // second level cache
                registry.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfig;
                });

                // disabled due to memory issues for large data set
                //var jsonSettings = new JsonSerializerSettings();
                //JsonConfiguration.Configure(jsonSettings);
                //registry.AddFusionCacheNewtonsoftJsonSerializer(jsonSettings);

                var textJsonSettings = new JsonSerializerOptions();
                TextJsonConfiguration.Configure(textJsonSettings);
                registry.AddFusionCacheSystemTextJsonSerializer(textJsonSettings);

                // backplane cache
                registry.AddFusionCacheStackExchangeRedisBackplane(options =>
                {
                    options.Configuration = redisConfig;
                });

                registry.Configure<FusionCacheOptions>(options =>
                {
                    options.DefaultEntryOptions = new FusionCacheEntryOptions
                    {
                        Duration = fusionConfig.Duration,
                        Priority = fusionConfig.Priority,
                    };
                });

                registry.AddFusionCache().TryWithAutoSetup();

            }
            else
            {
                registry.For<IFusionCache>().Use(Substitute.For<IFusionCache>()).Singleton();
            }

            // Get Honeycomb options from configuration
            var honeycombOptions = new HoneycombOptions
            {
                ServiceName = configuration["Honeycomb:ServiceName"] ?? "Flowchart-Builder-Default",
                ApiKey = configuration["Honeycomb:ApiKey"],
            };

            // Enable HTTP/2 support
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            registry.AddHttpContextAccessor();

            // Configure OpenTelemetry with Honeycomb
            // disabled due to memory and performance issues
            /*registry.AddOpenTelemetry()
                .WithMetrics(builder =>
                {
                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter()
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        .AddOtlpExporter(option =>
                        {
                            option.Endpoint = new Uri("https://api.honeycomb.io/v1/metrics");
                            option.Headers = $"x-honeycomb-team={honeycombOptions.ApiKey}";
                            option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        });
                });
            registry.AddOpenTelemetry().WithTracing(builder =>
            {
                builder
                .SetSampler(new AlwaysOnSampler())
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(honeycombOptions.ServiceName)
                    )
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddNpgsql()
                    .AddOtlpExporter(option =>
                    {
                        option.Endpoint = new Uri("https://api.honeycomb.io/v1/traces");
                        option.Headers = $"x-honeycomb-team={honeycombOptions.ApiKey}";
                        option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
            });

            // Register a Tracer provider factory
            registry.For<Tracer>().Use(context =>
            {
                return TracerProvider.Default.GetTracer(honeycombOptions.ServiceName);
            }).Singleton();*/
        }
    }
}