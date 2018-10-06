using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> IfNotNull<T>(
            this IQueryable<T> queryable,
            object checkObj,
            Func<IQueryable<T>, IQueryable<T>> ifNotNullQueryable)
            => If(queryable, checkObj != null, ifNotNullQueryable);

        public static IQueryable<T> IfNotNull<T, TS>(
            this IQueryable<T> queryable,
            TS? checkObj,
            Func<IQueryable<T>, IQueryable<T>> ifNotNullQueryable) where TS : struct
            => If(queryable, checkObj.HasValue, ifNotNullQueryable);

        public static IQueryable<T> If<T>(
            this IQueryable<T> queryable,
            bool check,
            Func<IQueryable<T>, IQueryable<T>> ifTrueQueryable)
            => check ? ifTrueQueryable(queryable) : queryable;

        public static IQueryable<T> ResetToDefault<T, TV>(
            this IQueryable<T> source,
            Func<TV, bool> predicate,
            ref TV value,
            TV defaultValue)
        {
            if (predicate(value))
                value = defaultValue;
            return source;
        }

        public static IQueryable<T> Variable<T, TV>(
            this IQueryable<T> source,
            out TV variable,
            Func<Task<TV>> variableCreator
        )
        {
            variable = variableCreator().Result;
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

        public static IQueryable<T> ForAll<T, TV>(
            this IQueryable<T> source,
            IEnumerable<TV> forSource,
            Func<IQueryable<T>, TV, IQueryable<T>> func) => forSource.Aggregate(source, func);

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool condition, Expression<Func<T, bool>> expression)
            => condition ? queryable.Where(expression) : queryable;
    }
}
