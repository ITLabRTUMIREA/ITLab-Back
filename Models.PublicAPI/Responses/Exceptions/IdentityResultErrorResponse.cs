using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.PublicAPI.Responses.Exceptions
{
    public class IdentityResultErrorResponse : ExceptionResponse
    {
        public List<IdentityError> Errors { get; }
        public IdentityResultErrorResponse(
            ResponseStatusCode statusCode,
            IEnumerable<IdentityError> errors,
            string message = null
            ) : base(statusCode, message)
        {
            Errors = errors.ToList();
        }

        public static IdentityResultErrorResponse Create(IEnumerable<IdentityError> errors, ResponseStatusCode statusCode)
            => new IdentityResultErrorResponse(statusCode, errors);
    }
}
