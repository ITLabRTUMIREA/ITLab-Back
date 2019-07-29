using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackEnd.Exceptions
{

    public class ApiLogicExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiLogicExceptionsHandlerMiddleware> logger;
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public ApiLogicExceptionsHandlerMiddleware(
            RequestDelegate next,
            IOptions<JsonSerializerSettings> jsonSerializerSettings,
            ILogger<ApiLogicExceptionsHandlerMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
            this.jsonSerializerSettings = jsonSerializerSettings.Value;
            this.jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException invalidOperationException) when (invalidOperationException.Message.Contains("SPA"))
            {
                context.Response.StatusCode = 404;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "error in application");
                context.Response.StatusCode = 500;
                if (ex is ApiLogicException apiEx)
                {
                    context.Response.StatusCode = (int)apiEx.StatusCode;
                }
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiLogicExceptionsHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLogicExceptionsHandlerMiddleware>();
        }
    }
}
