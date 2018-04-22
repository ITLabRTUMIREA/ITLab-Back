using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;

namespace BackEnd.Exceptions
{

    public class ApiLogicExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public ApiLogicExceptionsHandlerMiddleware(
            RequestDelegate next,
            IOptions<JsonSerializerSettings> jsonSerializerSettings)
        {
            _next = next;
            this.jsonSerializerSettings = jsonSerializerSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(Content(ex));

            }
        }
        private string Content(Exception ex)
            => JsonConvert.SerializeObject(GetData(ex), Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
        private object GetData(Exception ex)
        {
            switch (ex)
            {
                case ApiLogicException api:
                    return api.ResponseModel;
                case NotImplementedException nie:
                    return new ResponseBase(ResponseStatusCode.NotImplenment);
                default:
                    return new ResponseBase(ResponseStatusCode.Unknown);
            }
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiLogicExceptionsHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLogicExceptionsHandlerMiddleware>();
        }
    }
}
