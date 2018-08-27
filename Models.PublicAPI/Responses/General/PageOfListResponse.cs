using System;
using System.Collections.Generic;
using System.Data;
namespace Models.PublicAPI.Responses.General
{
	public class PageOfListResponse<T> : ListResponse<T>
    {
        public int Offset { get; set; }

    }
    public static class PageOfListExtensions
    {
        public static PageOfListResponse<T> ToPage<T>(this IEnumerable<T> list, int offset)
            => new PageOfListResponse<T> { Data = list, Offset = offset };
    }
}
