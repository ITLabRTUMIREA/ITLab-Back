using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class IQuerableExtensions
    {
        public static IQueryable<T> IfNotNull<T>(
            this IQueryable<T> querable,
            object checkObj,
            Func<IQueryable<T>, IQueryable<T>> ifNotNullQuearble)
            => If(querable, checkObj != null, ifNotNullQuearble);

        public static IQueryable<T> IfNotNull<T, S>(
            this IQueryable<T> querable,
            S? checkObj,
            Func<IQueryable<T>, IQueryable<T>> ifNotNullQuearble) where S : struct
            => If(querable, checkObj.HasValue, ifNotNullQuearble);

        public static IQueryable<T> If<T>(
            this IQueryable<T> querable,
            bool check,
            Func<IQueryable<T>, IQueryable<T>> ifTrueQuearble)
            => check ? ifTrueQuearble(querable) : querable;

        public static IQueryable<T> ResetToDefault<T, V>(
            this IQueryable<T> source,
            Func<V, bool> predicate,
            ref V value,
            V defaultValue)
        {
            if (predicate(value))
                value = defaultValue;
            return source;
        }

        public static IQueryable<T> Translate<T, TI, TO>(this IQueryable<T> source, TI parameter, out TO result, Func<TI, TO> translateFunc)
        {
            result = translateFunc(parameter);
            return source;
        }
        public static IQueryable<T> Translate<T, TI, TO>(this IQueryable<T> source, TI parameter, out TO result, TO defaultValue, params (TI parameter, TO result)[] conditions)
        {
            result = defaultValue;
            foreach (var condition in conditions)
                if (condition.parameter.Equals(parameter))
                {
                    result = condition.result;
                    break;
                }
            return source;
        }

        public static IQueryable<T> ForAll<T, V>(
            this IQueryable<T> source,
            IEnumerable<V> forSource,
            Func<IQueryable<T>, V, IQueryable<T>> func) => forSource.Aggregate(source, func);
    }
}
