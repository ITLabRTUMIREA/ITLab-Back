using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;

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

        public static InputParameterIncorrectResponse Create(IEnumerable<IdentityError> errors)
            => new InputParameterIncorrectResponse(
                errors
                    .Select(y =>
                        new IncorrectingInfo
                        {
                            Fieldname = y.Code,
                            Messages = new List<string> { y.Description }
                        })
                .ToList());
        
    }
        public class IncorrectingInfo
        {
            public string Fieldname { get; set; }
            public List<string> Messages { get; set; }
        }
}
