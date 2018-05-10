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
		{
			return checkObj == null ? querable : ifNotNullQuearble(querable);
		}
		public static IQueryable<T> IfNotNull<T, S>(
            this IQueryable<T> querable,
			S? checkObj,
			Func<IQueryable<T>, IQueryable<T>> ifNotNullQuearble) where S : struct
        {
			return checkObj.HasValue ? ifNotNullQuearble(querable) : querable;
        }

    }
}
