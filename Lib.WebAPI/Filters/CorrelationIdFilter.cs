using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Filters
{
    /// <summary>
    /// CorrelationIdFilter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CorrelationIdFilter : IOperationFilter
    {
        private const string Description = "Correlation Id";
        private static string ExampleGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                In = ParameterLocation.Header,
                Name = CorrelationIdMiddleware.CorrelationIdHeader,
                Required = false,
                Description = Description,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString(ExampleGuid),
                },
            });

            foreach (var response in operation.Responses.Select(x => x.Value))
            {
                response.Headers ??= new Dictionary<string, OpenApiHeader>();

                response.Headers.Add(CorrelationIdMiddleware.CorrelationIdHeader, new OpenApiHeader
                {
                    Description = Description,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                    },
                });
            }
        }
    }
}