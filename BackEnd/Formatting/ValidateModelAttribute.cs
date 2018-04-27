using BackEnd.Exceptions;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Formatting
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
            {

                throw new InputParameterIncorrectResponse(modelState.Select(kvp => new IncorrectingInfo
                    {
                        Fieldname = kvp.Key,
                        Messages = kvp.Value.Errors.Select(E => E.ErrorMessage).ToList()
                    }).ToList()
                    ).ToApiException
                    ();
            }
        }
    }
}
