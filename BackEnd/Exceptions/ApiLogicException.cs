using System;
using Models.PublicAPI.Responses;

namespace BackEnd.Exceptions
{
    public abstract class ApiLogicException : Exception
    {
        public ExceptionResponse ResponseModel { get; }

        protected ApiLogicException(ExceptionResponse responseBase, string message = null)
            : base(message)
        {
            ResponseModel = responseBase;
        }

    }
}
