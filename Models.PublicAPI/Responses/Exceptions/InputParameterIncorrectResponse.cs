using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Exceptions
{
    public class InputParameterIncorrectResponse : ExceptionResponse
    {
        public List<IncorrectingInfo> IncorrectFields { get; }

        public InputParameterIncorrectResponse(
            List<IncorrectingInfo> incorrectFields,
            string message = null
            ) : base(ResponseStatusCode.IncorrectRequestData, message)
        {
            IncorrectFields = incorrectFields;
        }
    }
        public class IncorrectingInfo
        {
            public string Fieldname { get; set; }
            public List<string> Messages { get; set; }
        }
}
