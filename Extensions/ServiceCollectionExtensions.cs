using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransient<T, TV, DTV>(this IServiceCollection collection)
            where T : class
            where TV : class, T
            where DTV : class, T
        {
#if DEBUG
            collection.AddTransient<T, TV>();
#else
            collection.AddTransient<T, DTV>();
#endif
            return collection;
        }
    }
}