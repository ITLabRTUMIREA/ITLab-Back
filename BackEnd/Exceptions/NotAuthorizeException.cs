using System;
using Models.PublicAPI.Responses;

namespace BackEnd.Exceptions
{
    public class NotAuthorizeException : ApiLogicException
    {
        public NotAuthorizeException() : base (ResponseStatusCode.NotAuthorize)
        {}
    }
}
