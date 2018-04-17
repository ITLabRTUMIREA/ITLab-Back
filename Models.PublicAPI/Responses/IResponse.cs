using System;
namespace Models.PublicAPI.Responses
{
    public interface IResponse
    {
        ResponseStatusCode StatusCode { get; }
    }
}
