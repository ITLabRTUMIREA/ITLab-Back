using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Models.PublicAPI.Responses;
using Newtonsoft.Json;

namespace BackEnd.Exceptions
{

    public class ApiLogicExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings jsonSerializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            //TypeNameHandling = TypeNameHandling.All
        };
        public ApiLogicExceptionsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
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
            => JsonConvert.SerializeObject(GetData(ex), Newtonsoft.Json.Formatting.Indented, jsonSerializeSettings);
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
