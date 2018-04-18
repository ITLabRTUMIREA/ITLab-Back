using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Equipments;

namespace BackEnd.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options): base(options)
        {
        }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
    }
}
