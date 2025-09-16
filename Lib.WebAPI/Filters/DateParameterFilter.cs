using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Filters
{
    /// <summary>
    /// DateParameterFilter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DateParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parms = operation.Parameters?.Where(x => x.In == ParameterLocation.Path && x.Schema?.Format?.Equals("date-time") == true).ToList();

            if (parms?.Any() == true)
            {
                foreach (var parm in parms)
                {
                    parm.Schema.Format = "string";
                    parm.Schema.Pattern = @"^\d{4}\-(0[1-9]|1[012])\-(0[1-9]|[12][0-9]|3[01])$";
                    parm.Schema.Example = new OpenApiString($"{DateTime.UtcNow:yyyy-MM-dd}");
                }
            }
        }
    }
}