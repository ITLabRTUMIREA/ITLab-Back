using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses
{
    public class ExceptionResponse : ResponseBase
    {
        public string Message { get; }
        public ExceptionResponse(ResponseStatusCode statusCode, string message = null) : base(statusCode)
        {
            Message = message;
        }
    }
}
