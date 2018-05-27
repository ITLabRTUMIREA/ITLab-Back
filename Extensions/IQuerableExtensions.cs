using System;
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
    }
}
