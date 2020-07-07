using BackEnd.DataBase;
using Exceptions.EquipmentExceptions;
using Exceptions.EquipmentTypeExceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Models.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.EquipmentServices
{
    public class EquipmentService
    {
        private readonly DataBaseContext dbContext;

        public EquipmentService(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Equipment> AddEquipment(
            Equipment newEquipment,
            List<Guid> childrenIds,
            Guid creatorId)
        {
            try
            {
                var lastNumber = await dbContext.Equipments
                    .Where(eq => eq.EquipmentTypeId == newEquipment.EquipmentTypeId)
                    .Select(eq => eq.Number)
                    .OrderByDescending(n => n)
                    .FirstOrDefaultAsync();

                if (childrenIds?.Count > 0)
                    newEquipment.Children =
                        await dbContext
                        .Equipments
                        .Where(eq => childrenIds.Contains(eq.Id))
                        .ToListAsync();

                if (newEquipment.Children?.Count != childrenIds?.Count)
                    throw new InvalidChildrenIdsException();

                await dbContext.Equipments.AddAsync(newEquipment);

                await dbContext.EquipmentOwnerChanges.AddAsync(new EquipmentOwnerChangeRecord
                {
                    NewOwnerId = null,
                    GranterId = creatorId,
                    ChangeOwnerTime = DateTime.UtcNow,
                    Equipment = newEquipment
                });

                newEquipment = await CreateEquipmentWithNumber(newEquipment, lastNumber);

                return newEquipment;
            }
            catch (DbUpdateException ex) when
                ((ex.InnerException is Npgsql.PostgresException pex) &&
                (pex.SqlState == "23505") && // unique_violation
                (pex.Detail.Contains(nameof(Equipment.SerialNumber))))
            {
                throw new SerialNumberExistsException(newEquipment.SerialNumber, ex);
            }
            catch (DbUpdateException ex) when
                ((ex.InnerException is Npgsql.PostgresException pex) &&
                (pex.SqlState == "23504") && // foreign_key_violation
                (pex.Detail.Contains(nameof(Equipment.EquipmentTypeId))))
            {
                throw new EquipmentTypeNotFoundException(newEquipment.EquipmentTypeId, ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<Equipment> CreateEquipmentWithNumber(Equipment equipment, int number)
        {
            try
            {
                equipment.Number = number;
                await dbContext.SaveChangesAsync();
                return equipment;
            }
            catch (DbUpdateException ex) when
                ((ex.InnerException is Npgsql.PostgresException pex) &&
                (pex.SqlState == "23505") && // unique_violation
                pex.Detail.Contains(nameof(Equipment.Number)))
            {
                return await CreateEquipmentWithNumber(equipment, number + 1);
            }
        }
    }
}
