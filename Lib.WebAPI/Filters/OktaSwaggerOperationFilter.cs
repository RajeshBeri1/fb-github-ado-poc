using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Filters
{
    /// <summary>
    /// OktaSwaggerOperationFilter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OktaSwaggerOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = context.MethodInfo.CustomAttributes.Any(x => x.AttributeType == typeof(AuthorizeAttribute)) ||
                context.MethodInfo.DeclaringType?.CustomAttributes.Any(x => x.AttributeType == typeof(AuthorizeAttribute)) == true;

            var isAnonymous = context.MethodInfo.CustomAttributes.Any(x => x.AttributeType == typeof(AllowAnonymousAttribute));

            if (isAuthorized && !isAnonymous)
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();

                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = OktaDefaults.ApiAuthenticationScheme,
                    },
                };
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>(),
                });
            }
        }
    }
}