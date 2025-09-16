using AutoMapper;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Security.Authentication;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// AspNetCoreExceptionHandler.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExceptionHandler
    {
        private readonly ILog<ExceptionHandler> log;
        private readonly IMapper mapper;

        /// <summary>
        /// Gets the supported exceptions.
        /// </summary>
        /// <value>The supported exceptions.</value>
        public static Dictionary<HttpStatusCode, (Type Type, LogLevel LogLevel)> SupportedExceptions { get; } =
            new Dictionary<HttpStatusCode, (Type, LogLevel)>()
            {
                { HttpStatusCode.NotFound, (typeof(KeyNotFoundException), LogLevel.Warning) },
                { HttpStatusCode.Unauthorized, (typeof(AuthenticationException), LogLevel.Error) },
                { HttpStatusCode.BadRequest, (typeof(ArgumentException), LogLevel.Warning) },
                { HttpStatusCode.InternalServerError, (typeof(Exception), LogLevel.Critical) },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="log">The log.</param>
        public ExceptionHandler(IMapper mapper, ILog<ExceptionHandler> log)
        {
            this.mapper = mapper;
            this.log = log;
        }

        /// <summary>
        /// Handles the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public IActionResult Handle(ActionContext context)
        {
            var errors = context.ModelState.Where(x => x.Value != null).Select(
                x => $"{x.Key}: {string.Join($" ", x.Value!.Errors.Select(e => e.ErrorMessage))}");

            var errorMessages = string.Join($" ", errors);

            log.Add(LogLevel.Warning, errorMessages);

            return new BadRequestObjectResult(new ExceptionDTO { Message = errorMessages });
        }

        /// <summary>
        /// Handles the asynchronous.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="context">The context.</param>
        public async Task HandleAsync(Exception exception, HttpContext context)
        {
            Activity currentActivity = Activity.Current;
            if (currentActivity != null)
            {
                currentActivity.SetTag("original_exception.type", exception.GetType().FullName);
                currentActivity.SetTag("original_exception.message", exception.Message);
                currentActivity.SetTag("original_exception.stacktrace", exception.StackTrace);
                // Add inner exception details if they exist.
                if (exception.InnerException != null)
                {
                    currentActivity.SetTag("original_exception.inner_type", exception.InnerException.GetType().FullName);
                    currentActivity.SetTag("original_exception.inner_message", exception.InnerException.Message);
                }
                currentActivity.RecordException(exception);
            }

            var content = mapper.Map<ExceptionDTO>(exception);
            var entry = SupportedExceptions.First(x => x.Value.Type.IsAssignableFrom(exception.GetType()));

            context.Response.StatusCode = (int)entry.Key;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            log.Add(entry.Value.LogLevel, exception.Message, exception);

            var json = Json.Serialize(content);

            await context.Response.WriteAsync(json, context.RequestAborted);
        }
    }
}