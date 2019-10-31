using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Models.PublicAPI.Requests;

namespace BackEnd.Formatting
{
    public class ListsConverter<T, TV> : ITypeConverter<List<T>, List<TV>>
    {
        private readonly Func<T, Guid> getTId;
        private readonly Func<TV, Guid> getTvId;
        private readonly Func<T, bool> needDelete;
        private readonly Func<T, TV, bool> isNeed;

        public ListsConverter(Func<TV, Guid> getTvId, Func<T, Guid> getTId = null, Func<T, TV, bool> isNeed = null, Func<T, bool> needDelete = null)
        {
            this.getTvId = getTvId;
            this.getTId = getTId ?? (source => source is IdRequest idReq ? idReq.Id : throw new ArgumentException($"item {source} [{source.GetType()}] should be {typeof(IdRequest)} or func {nameof(getTId)} should be passed"));
            
            this.isNeed = isNeed;
            this.needDelete = needDelete ?? (source => source is DeletableRequest deletable ? deletable.Delete : throw new ArgumentException($"item {source} [{source.GetType()}] should be {typeof(DeletableRequest)} or func {nameof(needDelete)} should be passed"));
        }


        public List<TV> Convert(List<T> source, List<TV> destination, ResolutionContext context)
        {
            Console.WriteLine($"convert {typeof(T).Name} to {typeof(TV).Name}");
            destination = destination ?? new List<TV>();
            var toAdd = new List<TV>();
            foreach (var sourceItem in source)
            {
                if (needDelete(sourceItem))
                {
                    destination.RemoveAll(i => getTvId(i) == getTId(sourceItem));
                    continue;
                }
                var destItem = destination.FirstOrDefault(i => getTvId(i) == getTId(sourceItem));

                if (isNeed?.Invoke(sourceItem, destItem) == false)
                    continue;

                if (destItem != null)
                {
                    context.Mapper.Map(sourceItem, destItem);
                }
                else
                {
                    toAdd.Add(context.Mapper.Map<TV>(sourceItem));
                }
            }
            destination.AddRange(toAdd);
            return destination;
        }
    }
}