using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Lib.Athena.Models;
using Lib.Common.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Lib.Athena.Business
{
    /// <summary>
    /// RoleBasedAWSCredentials
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RoleBasedAWSCredentials : AWSCredentials
    {
        private readonly IOptionsMonitor<AWSCredentialConfig> credentialConfig;
        private readonly AWSOptions options;
        private readonly IAsyncPolicy<AssumeRoleResponse> retryPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBasedAWSCredentials" />
        /// class.
        /// </summary>
        /// <param name="credentialConfig">The credential configuration.</param>
        /// <param name="options">The options.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="queryConfig">The query configuration.</param>
        /// <param name="log">The log.</param>
        public RoleBasedAWSCredentials(
            IOptionsMonitor<AWSCredentialConfig> credentialConfig,
            AWSOptions options,
            IConfigurationRoot configuration,
            IOptionsMonitor<AthenaQueryConfig> queryConfig,
            ILog<RoleBasedAWSCredentials> log)
        {
            this.credentialConfig = credentialConfig;
            this.options = options;

            retryPolicy = Policy<AssumeRoleResponse>
                        .Handle<Exception>()
                        .OrResult(x => x.HttpStatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
                        .WaitAndRetryAsync(
                            Backoff.AwsDecorrelatedJitterBackoff(queryConfig.CurrentValue.RetryMinDelay, queryConfig.CurrentValue.RetryMaxDelay, queryConfig.CurrentValue.RetryCount),
                            onRetry: (result, timeSpan) =>
                                {
                                    log.Add(LogLevel.Warning, $"Retrying Athena connection, retry in: {timeSpan}, reason: {result.Exception?.Message}");

                                    var accessKey = credentialConfig.CurrentValue.AccessKey;
                                    var secretKey = credentialConfig.CurrentValue.SecretKey;

                                    log.Add(LogLevel.Information, $"Reloading configuration settings.");

                                    configuration.Reload();

                                    if (accessKey != credentialConfig.CurrentValue.AccessKey || secretKey != credentialConfig.CurrentValue.SecretKey)
                                    {
                                        log.Add(LogLevel.Information, $"Access key or secret key has been changed.");
                                    }
                                });
        }

        /// <summary>
        /// Returns a copy of ImmutableCredentials
        /// </summary>
        public override ImmutableCredentials GetCredentials() => GetCredentialsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Gets the credentials asynchronous.
        /// </summary>
        public override async Task<ImmutableCredentials> GetCredentialsAsync()
        {
            var response = await retryPolicy.ExecuteAsync(() => AssumeRoleAsync());

            return new ImmutableCredentials(response.Credentials.AccessKeyId, response.Credentials.SecretAccessKey, response.Credentials.SessionToken);
        }

        private async Task<AssumeRoleResponse> AssumeRoleAsync()
        {
            var credentials = new BasicAWSCredentials(credentialConfig.CurrentValue.AccessKey, credentialConfig.CurrentValue.SecretKey);
            using var client = new AmazonSecurityTokenServiceClient(credentials, options.Region);

            return await client.AssumeRoleAsync(new AssumeRoleRequest
            {
                DurationSeconds = credentialConfig.CurrentValue.DurationSeconds,
                ExternalId = credentialConfig.CurrentValue.ExternalId,
                RoleSessionName = credentialConfig.CurrentValue.RoleSessionName,
                RoleArn = credentialConfig.CurrentValue.RoleArn,
            });
        }
    }
}