using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.General
{
    public class ListResponse<T> : ResponseBase
    {
        public IEnumerable<T> Data { get; set; }

        public ListResponse(){}
        public ListResponse(
            ResponseStatusCode statusCode,
            IEnumerable<T> data) : base(statusCode)
        {
            Data = data;
        }
        public static ListResponse<T> Create(IEnumerable<T> data)
            => new ListResponse<T>(ResponseStatusCode.OK, data);

        public static implicit operator ListResponse<T>(List<T> value)
            => Create(value);
    }
}
