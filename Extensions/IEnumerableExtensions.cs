using System;
using System.Collections.Generic;
using System.Collections;
namespace Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> DoForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
            return source;
        }
    }
}
