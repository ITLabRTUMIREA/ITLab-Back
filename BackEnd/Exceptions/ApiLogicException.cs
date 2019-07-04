using System;
using System.Net;
using Models.PublicAPI.Responses;

namespace BackEnd.Exceptions
{
    public class ApiLogicException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ApiLogicException(string message = "", HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public static ApiLogicException NotFound(string message = "") => new ApiLogicException(message, HttpStatusCode.NotFound);

        internal static ApiLogicException Conflict(string message = "") => new ApiLogicException(message, HttpStatusCode.Conflict);
    }
}
