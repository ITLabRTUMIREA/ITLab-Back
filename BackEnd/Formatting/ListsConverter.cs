using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Models.PublicAPI.Requests;
using StackExchange.Redis;

namespace BackEnd.Formatting
{
    public class ListsConverter<T, V> : ITypeConverter<List<T>, List<V>>
        where T:DeletableRequest
        
    {
        private readonly Func<V, Guid> getId;

        public ListsConverter(Func<V, Guid> getId)
        {
            this.getId = getId;
        }
        public List<V> Convert(List<T> source, List<V> destination, ResolutionContext context)
        {
            Console.WriteLine($"convert {typeof(T).Name} to {typeof(V).Name}");
            destination = destination ?? new List<V>();
            foreach (var sourceItem in source)
            {
                if (sourceItem.Delete)
                {
                    destination.RemoveAll(i => getId(i) == sourceItem.Id);
                    continue;
                }
                var destItem = destination.FirstOrDefault(i => getId(i) == sourceItem.Id);
                if (destItem != null)
                {
                    context.Mapper.Map(sourceItem, destItem);
                }
                else
                {
                    destination.Add(context.Mapper.Map<V>(sourceItem));
                }
            }
            return destination;
        }
    }
}