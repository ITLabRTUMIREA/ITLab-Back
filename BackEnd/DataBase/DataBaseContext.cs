using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Equipments;
using Models.Events;
using Models;

namespace BackEnd.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
