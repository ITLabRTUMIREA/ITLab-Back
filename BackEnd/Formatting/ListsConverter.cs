using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Extensions.General;
using Models.PublicAPI.Requests;
using StackExchange.Redis;

namespace BackEnd.Formatting
{
    public class ListsConverter<T, V> : ITypeConverter<List<T>, List<V>>
        where T:DeletableRequest 
        where V:IdInterface
        
    {
        public List<V> Convert(List<T> source, List<V> destination, ResolutionContext context)
        {
            destination = destination ?? new List<V>();
            foreach (var sourceItem in source)
            {
                if (sourceItem.Delete)
                    destination.RemoveAll(i => i.Id == sourceItem.Id);
                var destItem = destination.FirstOrDefault(i => i.Id == sourceItem.Id);
                if (destItem != null)
                    context.Mapper.Map(sourceItem, destItem);
            }
            return destination;
        }
    }
}