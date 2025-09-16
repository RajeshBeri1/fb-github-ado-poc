using System.Net;
using Amazon.Athena;
using Amazon.Athena.Model;
using Amazon.Runtime;
using Lib.Athena.Business.Interfaces;
using Lib.Athena.Models;
using Lib.Common.Business.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Lib.Athena.Business
{
    /// <summary>
    /// AthenaQueryLogic
    /// </summary>
    public class AthenaQueryLogic : IAthenaQueryLogic
    {
        private static readonly string[] RunningStates = { QueryExecutionState.QUEUED, QueryExecutionState.RUNNING };

        private readonly IAmazonAthena athena;
        private readonly IOptionsMonitor<AthenaQueryConfig> athenaQueryConfig;
        private readonly ILog<IAthenaQueryLogic> log;
        private readonly IAsyncPolicy<GetQueryResultsResponse> retryPolicyResults;
        private readonly IAsyncPolicy<StartQueryExecutionResponse> retryPolicyStart;
        private readonly IAsyncPolicy<GetQueryExecutionResponse> retryPolicyWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="AthenaQueryLogic" /> class.
        /// </summary>
        /// <param name="athena">The athena.</param>
        /// <param name="athenaQueryConfig">The athena query configuration.</param>
        /// <param name="log">The log.</param>
        public AthenaQueryLogic(IAmazonAthena athena, IOptionsMonitor<AthenaQueryConfig> athenaQueryConfig, ILog<IAthenaQueryLogic> log)
        {
            this.athena = athena;
            this.athenaQueryConfig = athenaQueryConfig;
            this.log = log;

            retryPolicyStart = CreateRetryPolicy<StartQueryExecutionResponse>(athenaQueryConfig);
            retryPolicyWait = CreateRetryPolicy<GetQueryExecutionResponse>(athenaQueryConfig);
            retryPolicyResults = CreateRetryPolicy<GetQueryResultsResponse>(athenaQueryConfig);
        }

        /// <summary>
        /// Gets the table column infos asynchronous.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<ColumnInfo>> GetTableColumnInfosAsync(string table, CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM {table} LIMIT 1";
            var result = await QueryAsync(query, cancellationToken);
            return result.ColumnInfo;
        }

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<AthenaQueryResult> QueryAsync(string query, CancellationToken cancellationToken)
        {
            var id = await StartQueryExecutionAsync(query, cancellationToken);
            await WaitForQueryCompletionAsync(id, cancellationToken);
            return await GetQueryResultsAsync(id, cancellationToken);
        }

        private IAsyncPolicy<T> CreateRetryPolicy<T>(IOptionsMonitor<AthenaQueryConfig> queryConfig) where T : AmazonWebServiceResponse
            => Policy<T>.Handle<Exception>()
                        .OrResult(x => x.HttpStatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
                        .WaitAndRetryAsync(
                            Backoff.AwsDecorrelatedJitterBackoff(queryConfig.CurrentValue.RetryMinDelay, queryConfig.CurrentValue.RetryMaxDelay, queryConfig.CurrentValue.RetryCount),
                            onRetry: (result, timeSpan) =>
                            {
                                log.Add(LogLevel.Warning, $"Retrying Athena connection ({typeof(T).Name}), retry in: {timeSpan}, reason: {result.Exception?.Message}");
                            });

        private async Task<AthenaQueryResult> GetQueryResultsAsync(string id, CancellationToken cancellationToken)
        {
            var request = new GetQueryResultsRequest
            {
                MaxResults = athenaQueryConfig.CurrentValue.MaxResults,
                QueryExecutionId = id,
            };

            var result = new AthenaQueryResult();

            do
            {
                var response = await retryPolicyResults.ExecuteAsync(() => athena.GetQueryResultsAsync(request, cancellationToken));

                result.ColumnInfo ??= response.ResultSet.ResultSetMetadata.ColumnInfo;

                result.Rows.AddRange(response.ResultSet.Rows);

                request.NextToken = response.NextToken;
            }
            while (request.NextToken != null);

            return result;
        }

        private async Task<string> StartQueryExecutionAsync(string query, CancellationToken cancellationToken)
        {
            var request = new StartQueryExecutionRequest
            {
                WorkGroup = athenaQueryConfig.CurrentValue.WorkgroupName,
                QueryString = query,
                QueryExecutionContext = new QueryExecutionContext
                {
                    Database = athenaQueryConfig.CurrentValue.DatabaseName,
                    Catalog = athenaQueryConfig.CurrentValue.CatalogName,
                },
                ResultConfiguration = new ResultConfiguration
                {
                    OutputLocation = athenaQueryConfig.CurrentValue.OutputLocation,
                },
            };

            log.Add(LogLevel.Information, $"Running Athena query: {query}");

            var response = await retryPolicyStart.ExecuteAsync(() => athena.StartQueryExecutionAsync(request, cancellationToken));

            return response.QueryExecutionId;
        }

        private async Task WaitForQueryCompletionAsync(string id, CancellationToken cancellationToken)
        {
            var request = new GetQueryExecutionRequest
            {
                QueryExecutionId = id,
            };

            while (true)
            {
                var result = await retryPolicyWait.ExecuteAsync(() => athena.GetQueryExecutionAsync(request, cancellationToken));

                if (!RunningStates.Any(x => x == result.QueryExecution.Status.State.Value))
                {
                    if (result.QueryExecution.Status.State == QueryExecutionState.FAILED)
                    {
                        throw new Exception($"Athena query failed: {result.QueryExecution.Status.AthenaError.ErrorMessage}");
                    }

                    break;
                }

                await Task.Delay(athenaQueryConfig.CurrentValue.StatusPollingDelay, cancellationToken);
            }
        }
    }
}