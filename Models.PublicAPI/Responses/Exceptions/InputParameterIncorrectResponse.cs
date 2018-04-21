using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Exceptions
{
    public class InputParameterIncorrectResponse : ExceptionResponse
    {
        public List<string> IncorrectFields { get; }

        public InputParameterIncorrectResponse(
            ResponseStatusCode statusCode,
            List<string> incorrectFields,
            string message = null
            ) : base(statusCode, message)
        {
            IncorrectFields = incorrectFields;
        }

    }
}
