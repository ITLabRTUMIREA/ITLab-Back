using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Extensions;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DataBaseLinks;
using Models.Events;
using Models.PublicAPI.Requests.Events.Event;
using Models.PublicAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
using System.Runtime.Versioning;
using Models.People;

namespace BackEnd.Services
{
    public class EventsManager : IEventsManager
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;

        public EventsManager(
            DataBaseContext dbContext,
            IMapper mapper
        )
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public IQueryable<Event> Events => dbContext.Events;

        public Task<Event> FindAsync(Guid id)
            => CheckAndGetEventAsync(id);

        public async Task<IQueryable<Event>> AddAsync(EventCreateRequest request)
        {
            await CheckAndGetEventTypeAsync(request.EventTypeId);
            var newEvent = mapper.Map<Event>(request);
            newEvent
                .Shifts
                .SelectMany(s => s.Places)
                .SelectMany(p => p.PlaceUserRoles)
                .WithActions(p =>
                {
                    p.UserStatus = UserStatus.Invited;
                    p.CreationTime = DateTime.UtcNow;
                })
                .Iterate();

            await dbContext.Events.AddAsync(newEvent);
            await dbContext.SaveChangesAsync();
            return dbContext.Events.Where(ev => ev.Id == newEvent.Id);
        }


        public async Task<IQueryable<Event>> EditAsync(EventEditRequest request)
        {
            var toEdit = await CheckAndGetEventAsync(request.Id);
            if (request.EventTypeId.HasValue)
                await CheckAndGetEventTypeAsync(request.EventTypeId.Value);

            mapper.Map(request, toEdit);

            toEdit
                .Shifts
                .SelectMany(s => s.Places)
                .SelectMany(p => p.PlaceUserRoles)
                .Where(p => p.UserStatus == UserStatus.Unknown)
                .WithActions(p =>
                {
                    p.UserStatus = UserStatus.Invited;
                    p.CreationTime = DateTime.UtcNow;
                })
                .Iterate();

            if (toEdit.Shifts?.Count < 1)
                throw ResponseStatusCode.LastShift.ToApiException();
            await dbContext.SaveChangesAsync();
            return dbContext.Events.Where(ev => ev.Id == toEdit.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await CheckAndGetEventAsync(id);
            dbContext.Events.Remove(toDelete);
            await dbContext.SaveChangesAsync();
        }

        private async Task<EventType> CheckAndGetEventTypeAsync(Guid typeId)
            => await dbContext.EventTypes.FindAsync(typeId)
               ?? throw ResponseStatusCode.EventTypeNotFound.ToApiException();

        private async Task<Event> CheckAndGetEventAsync(Guid id)
            => await dbContext.Events
                   .Include(e => e.EventType)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceEquipments)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceUserRoles)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceUserRoles)
                   .ThenInclude(pur => pur.Role)
                   .Include(e => e.Shifts)
                   .ThenInclude(s => s.Places)
                   .ThenInclude(p => p.PlaceUserRoles)
                   .ThenInclude(pur => pur.User)
                   .FirstOrDefaultAsync(e => e.Id == id)
                   ?? throw ResponseStatusCode.NotFound.ToApiException();

        public async Task WishTo(Guid userId, Guid roleId, Guid placeId)
        {
            var targetPlace = await dbContext
                .Events
                .SelectMany(e => e.Shifts)
                .SelectMany(s => s.Places)
                .Include(p => p.PlaceUserRoles)
                .SingleOrDefaultAsync(p => p.Id == placeId)
                ?? throw ResponseStatusCode.NotFound.ToApiException();

            var nowInRole = targetPlace
                .PlaceUserRoles
                .Any(pur => pur.UserId == userId);
            if (nowInRole)
                throw ResponseStatusCode.YouAreInRole.ToApiException();
            targetPlace.PlaceUserRoles.Add(new PlaceUserRole
            {
                UserId = userId,
                RoleId = roleId,
                UserStatus = UserStatus.Wisher,
                CreationTime = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task AcceptInvite(Guid placeId, Guid userId)
        {
            var targetPlaceUserRole = await FindPlaceUserRole(placeId, userId, UserStatus.Invited);
            targetPlaceUserRole.UserStatus = UserStatus.Accepted;
            targetPlaceUserRole.DoneTime = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        public async Task RejectInvite(Guid placeId, Guid userId)
        {
            var targetPlaceUserRole = await FindPlaceUserRole(placeId, userId, UserStatus.Invited);
            dbContext.Remove(targetPlaceUserRole);
            targetPlaceUserRole.DoneTime = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        public async Task AcceptWish(Guid placeId, Guid userId)
        {
            var targetPlaceUserRole = await FindPlaceUserRole(placeId, userId, UserStatus.Wisher);
            targetPlaceUserRole.UserStatus = UserStatus.Accepted;
            targetPlaceUserRole.DoneTime = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        public async Task RejectWish(Guid placeId, Guid userId)
        {
            var targetPlaceUserRole = await FindPlaceUserRole(placeId, userId, UserStatus.Wisher);
            dbContext.Remove(targetPlaceUserRole);
            targetPlaceUserRole.DoneTime = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        private async Task<PlaceUserRole> FindPlaceUserRole(Guid placeId, Guid userId, UserStatus status)
            => await PlaceUserRoles
                .SingleOrDefaultAsync(pur => pur.PlaceId == placeId && pur.UserId == userId && pur.UserStatus == status)
                ?? throw ResponseStatusCode.NotFound.ToApiException();

        private IQueryable<PlaceUserRole> PlaceUserRoles =>
            dbContext
                .Events
                .SelectMany(e => e.Shifts)
                .SelectMany(s => s.Places)
                .SelectMany(p => p.PlaceUserRoles);
    }
}