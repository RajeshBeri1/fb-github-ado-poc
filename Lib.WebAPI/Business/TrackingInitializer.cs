using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// Adds tracking properties (correlation ID, logged-in user, etc) to all logs
    /// </summary>
    public class TrackingInitializer : ITelemetryInitializer
    {
        private static readonly Func<string, Func<Exception>> AuthException = (x) => () => new AuthenticationException(x);
        private IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingInitializer" /> class.
        /// </summary>
        /// <param name="httpContextAccessor">The IHttpContextAccessor.</param>
        public TrackingInitializer(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                if (httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null && httpContextAccessor.HttpContext.User.Identity != null && httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
                    claimsIdentity.ThrowIfNull(AuthException($"Identity is not a claims identity"));

                    var personId = claimsIdentity.Claims?.FirstOrDefault(x => x.Type == OmniAuthenticationHandler.ClaimPersonId)?.Value;
                    Guid.TryParse(personId, out var userId).Throw().IfFalse();

                    string? userName = claimsIdentity?.Name;

                    telemetry.Context.User.Id = personId?.ToString();
                    telemetry.Context.GlobalProperties.Add("UserName", userName);

                    // for logging IP Address and custom properties : To check on azure -use syntax: tostring(customDimensions["UserSystemIP"])
                    telemetry.Context.GlobalProperties.Add("UserSystemIP", telemetry.Context.Location.Ip);
                    telemetry.Context.GlobalProperties.Add("UtcTimeNow", DateTime.UtcNow.ToString());
                }
            }
            catch (Exception exception)
            {
                ((ISupportProperties)telemetry).Properties.Add("TRACKING_INITIALIZER_EXCEPTION", exception.ToString());
            }
        }
    }
}