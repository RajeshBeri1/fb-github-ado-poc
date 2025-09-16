using Hangfire;
using Hangfire.InMemory;
using Lamar.Microsoft.DependencyInjection;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business.Interfaces;
using Lib.SampleData.Business;
using Lib.WebAPI.Business;
using Lib.WebAPI.Configuration;
using Lib.WebAPI.Models;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Load appsettings.override.json
builder.Configuration.AddJsonFile("appsettings.override.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Host.UseLamar(registry => LamarConfiguration.Configure(registry, builder.Configuration, builder.Environment));

// disable the honeycomb exporter for now due to memory issues
/*builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.AddConsoleExporter();

});
*/
var useSampleData = builder.Configuration.GetValue<bool>("UseSampleData");

if (useSampleData || builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<SampleDataDbContext>();
    builder.Services.AddTransient<IAuroraQueryLogic, AthenaSampleDataQueryLogic>();
}

// Get connection string from appsettings.override.json
//var connectionString = builder.Configuration.GetConnectionString("PortalDB");

builder.Services.AddHangfire(x => x
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage(new InMemoryStorageOptions
        {
            MaxExpirationTime = TimeSpan.FromHours(3),
            IdType = InMemoryStorageIdType.Long,

        }));
//.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer(c => c.WorkerCount = Environment.ProcessorCount * 5);
#region // Rate limiter code start here
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(300);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });
});
#endregion // Rate limiter code end here

var app = builder.Build();

app.UseResponseCompression();

var log = app.Services.GetRequiredService<ILog<Program>>();
using var source = new CancellationTokenSource();

app.UseSwagger();

#if DEBUG
app.UseSwaggerUI(); // Only enable Swagger UI in debug mode
#endif
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    log.Add(LogLevel.Information, $"Creating storage containers.");

    var storage = app.Services.GetRequiredService<AzureBlobStorage>();
    var config = app.Services.GetRequiredService<IOptionsMonitor<StorageConfig>>();
    await storage.CreateContainerAsync(config.CurrentValue.ContainerName, source.Token);
}

if (useSampleData || builder.Environment.IsDevelopment())
{
    log.Add(LogLevel.Information, $"Creating sample data database.");

    var sampleDb = app.Services.GetRequiredService<SampleDataDbContext>();
    await sampleDb.EnsureCreatedAsync(source.Token);
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseHangfireDashboard();

app.UseCors(builder =>
{
    var allowedOrigins = app.Configuration.GetSection(nameof(HostingConfig)).GetValue<string>(nameof(HostingConfig.AllowedOrigins));
    if (allowedOrigins != null)
    {
        builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(allowedOrigins.Split(","))
        .AllowCredentials()
        .SetPreflightMaxAge(TimeSpan.FromMinutes(30));
    }
});

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();