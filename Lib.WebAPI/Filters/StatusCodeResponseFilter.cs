using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Lib.WebAPI.Business;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Filters
{
    /// <summary>
    /// StatusCodeResponseFilter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StatusCodeResponseFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var mediaType = new OpenApiMediaType
            {
                Schema = context.SchemaGenerator.GenerateSchema(typeof(ExceptionDTO), context.SchemaRepository),
            };

            foreach (var ex in ExceptionHandler.SupportedExceptions)
            {
                var respone = new OpenApiResponse
                {
                    Description = ReasonPhrases.GetReasonPhrase((int)ex.Key),
                };

                respone.Content.Add(MediaTypeNames.Application.Json, mediaType);

                operation.Responses.Add($"{(int)ex.Key}", respone);
            }
        }
    }
}