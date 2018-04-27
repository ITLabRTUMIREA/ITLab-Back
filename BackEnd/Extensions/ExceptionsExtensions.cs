using BackEnd.Exceptions;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Extensions
{
    public static class ExceptionsExtensions
    {
        public static ApiLogicException ToApiException(this ExceptionResponse exceptionResponse)
            => new ApiLogicException(exceptionResponse);
        public static ApiLogicException ToApiException(this ResponseStatusCode statusCode)
            => ApiLogicException.Create(statusCode);

    }
}
