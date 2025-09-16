using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Filters
{
    /// <summary>
    /// CustomModelFilter
    /// </summary>
    public class CustomModelFilter<T> : IDocumentFilter
    {
        /// <summary>
        /// Applies the specified swagger document.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
        }
    }
}