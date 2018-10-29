using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DataBase;
using Extensions;
using Microsoft.EntityFrameworkCore;
using WebApp.Configure.Models.Configure.Interfaces;

namespace BackEnd.Services.ConfigureServices
{
    public class EquipmentUpgradeMigrate : IConfigureWork
    {
        private readonly DataBaseContext dbContext;
        public const string ConditionKey = nameof(EquipmentUpgradeMigrate);

        public EquipmentUpgradeMigrate(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Configure()
        {
            var equipmentTypes = await dbContext.EquipmentTypes.Include(et => et.Equipment).ToListAsync();
            equipmentTypes.ForEach(et => et.LastNumber = et.Equipment.Count);
            equipmentTypes.ForEach(et => et.Equipment.Select((e, i) =>
            {
                e.Number = i;
                return e;
            }).Iterate());
            await dbContext.SaveChangesAsync();

            var types = await dbContext
                .EventRoles
                .Include(u => u.PlaceUserEventRoles)
                .ToListAsync();
            var russianParticipantType = types.Single(t => t.Title == "Участник");
            var englishParticipant = types.SingleOrDefault(t => t.Title == "Participant");
            if (englishParticipant != null)
            {

                englishParticipant.PlaceUserEventRoles.ForEach(p =>
                {
                    p.EventRole = russianParticipantType;
                    p.EventRoleId = russianParticipantType.Id;
                });
                dbContext.Remove(englishParticipant);
            }
            var russianOrganizer = types.Single(t => t.Title == "Организатор");
            var englishOrganizer = types.SingleOrDefault(t => t.Title == "Organizer");
            if (englishOrganizer != null)
            {
                englishOrganizer.PlaceUserEventRoles.ForEach(p =>
                {
                    p.EventRole = russianOrganizer;
                    p.EventRoleId = russianOrganizer.Id;
                });
                dbContext.Remove(englishOrganizer);
            }

            await dbContext.SaveChangesAsync();
        }
    }

}

