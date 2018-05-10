using System;
namespace Models.PublicAPI.Responses
{
    public class ResponseBase
    {
        public ResponseStatusCode StatusCode { get; }
# if DEBUG
        public string OnlyDebugStatusCodeText { get; }
#endif
        public ResponseBase(ResponseStatusCode statusCode)
        {
            StatusCode = statusCode;
# if DEBUG
            OnlyDebugStatusCodeText = statusCode.ToString();
#endif

        }
        public static implicit operator ResponseBase(ResponseStatusCode statusCode)
            => new ResponseBase(statusCode);
    }
}
