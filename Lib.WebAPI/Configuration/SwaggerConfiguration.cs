using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Filters;
using Lib.WebAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lib.WebAPI.Configuration
{
    /// <summary>
    /// SwaggerConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configures the specified setup.
        /// </summary>
        /// <param name="setup">The setup.</param>
        /// <param name="omniAuthConfig">The omni authentication configuration.</param>
        /// <param name="oktaConfig">The okta configuration.</param>
        public static void Configure(SwaggerGenOptions setup, OmniAuthConfig omniAuthConfig, OktaConfig oktaConfig)
        {
            setup.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.ActionDescriptor.RouteValues["action"]}");

            // satisfy swagger-codegen-cli with proper object types
            setup.MapType<object>(() => new OpenApiSchema { Type = "object", Nullable = true });

            var xmlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            xmlFiles.ForEach(x => setup.IncludeXmlComments(x));

            setup.OperationFilter<StatusCodeResponseFilter>();
            setup.OperationFilter<ProducesSwaggerOperationFilter>();
            setup.OperationFilter<OktaSwaggerOperationFilter>();
            setup.OperationFilter<CorrelationIdFilter>();
            setup.OperationFilter<DateParameterFilter>();

            setup.DocumentFilter<CustomModelFilter<TaskpaneMode>>();

            if (omniAuthConfig.Enabled && !oktaConfig.Enabled)
            {
                setup.AddSecurityDefinition(OktaDefaults.ApiAuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "Okta Authentication",
                    Name = HeaderNames.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = OktaDefaults.ApiAuthenticationScheme,
                });
            }
            else
            {
                setup.AddSecurityDefinition(OktaDefaults.ApiAuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "Omni Authentication",
                    Name = HeaderNames.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = OmniAuthenticationHandler.AuthenticationScheme,
                });
            }
        }
    }
}