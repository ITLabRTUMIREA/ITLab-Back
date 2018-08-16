using System;
using System.Linq;
using BackEnd.Models;
using Models.Events;
using AutoMapper.QueryableExtensions;
using Models.PublicAPI.Responses.Event;
namespace BackEnd.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<CompactEventView> ToCompactEventView(this IQueryable<Event> source, Guid userId)
            => source.ProjectTo<EventAndUserId>(new { userId }).ProjectTo<CompactEventView>();
    }
}
