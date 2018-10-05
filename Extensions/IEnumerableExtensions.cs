using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

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

        public static IEnumerable<TV> SelectMany<T, TV>(
            this IEnumerable<T> source,
            Func<T, (TV, TV)> selector
        )
        => source.SelectMany(i => selector(i).ToEnumerable());


        private static IEnumerable<TV> ToEnumerable<TV>(this (TV, TV) source)
        {
            yield return source.Item1;
            yield return source.Item2;
        }
    }
}
