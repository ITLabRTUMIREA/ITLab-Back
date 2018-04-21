using System;
namespace Models.PublicAPI.Responses
{
    public class ResponseBase
    {
        public ResponseStatusCode StatusCode { get; }
# if DEBUG
        public string StatusCodeText { get; }
#endif
        public ResponseBase(ResponseStatusCode statusCode)
        {
            StatusCode = statusCode;
# if DEBUG
            StatusCodeText = statusCode.ToString();
#endif

        }
    }
}
