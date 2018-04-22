using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.General
{
    public class OneObjectResponse<T> : ResponseBase
    {
        public T Data { get; }
        public OneObjectResponse(
            ResponseStatusCode statusCode,
            T data) : base(statusCode)
        {
            Data = data;
        }
        public static OneObjectResponse<T> Create(T data)
            => new OneObjectResponse<T>(ResponseStatusCode.OK, data);

        public static implicit operator OneObjectResponse<T>(T value)
            => Create(value);
    }
}
