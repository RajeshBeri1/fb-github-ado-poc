using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Filters
{
    /// <summary>
    /// ProducesSwaggerOperationFilter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProducesSwaggerOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var attribute in context.MethodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(ProducesAttribute)))
            {
                var param = attribute.ConstructorArguments.FirstOrDefault();
                if (param.ArgumentType != typeof(string))
                {
                    continue;
                }

                var contentType = param.Value as string;

                foreach (var response in operation.Responses.Where(x => x.Key == $"{(int)HttpStatusCode.OK}").Select(x => x.Value))
                {
                    response.Content ??= new Dictionary<string, OpenApiMediaType>();

                    response.Content.Add(contentType, new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "file",
                            Description = "File response.",
                            Format = "binary",
                        },
                    });

                    response.Extensions ??= new Dictionary<string, IOpenApiExtension>();
                    response.Extensions.Add("x-is-file", new OpenApiBoolean(true));

                    response.Headers ??= new Dictionary<string, OpenApiHeader>();

                    response.Headers.Add("Content-Disposition", new OpenApiHeader
                    {
                        Description = "Content disposition header with filename.",
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                        },
                    });

                    response.Headers.Add("Content-Length", new OpenApiHeader
                    {
                        Description = "File length.",
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                        },
                    });
                }
            }
        }
    }
}