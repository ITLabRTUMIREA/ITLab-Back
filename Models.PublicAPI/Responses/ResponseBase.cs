using System;
namespace Models.PublicAPI.Responses
{
    public class ResponseBase : IResponse
    {
        public ResponseStatusCode StatusCode { get; }

        public ResponseBase(ResponseStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
