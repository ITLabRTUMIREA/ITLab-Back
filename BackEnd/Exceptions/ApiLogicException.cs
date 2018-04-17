using System;
using Models.PublicAPI.Responses;

namespace BackEnd.Exceptions
{
    public abstract class ApiLogicException : Exception, IResponse
    {
        public ResponseStatusCode StatusCode { get; set; }
        protected ApiLogicException(ResponseStatusCode statusCode, string message = null)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
