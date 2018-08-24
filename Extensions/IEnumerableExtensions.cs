using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.WebUtilities;

namespace Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> WithActions<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static void Iterate<T>(this IEnumerable<T> source)
        {
            foreach (var variable in source)
            {}
        }

    }
}
