using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Equipments;
using Models.Events;
using Models;
using Models.DataBaseLinks;

namespace BackEnd.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventEquipment>()
                .HasKey(t => new { t.EquipmentId, t.EventId });

            modelBuilder.Entity<EventEquipment>()
                .HasOne(ee => ee.Event)
                .WithMany(ev => ev.EventEquipments)
                .HasForeignKey(ee => ee.EventId);

            modelBuilder.Entity<EventEquipment>()
                .HasOne(ee => ee.Equipment)
                .WithMany(eq => eq.EventEquipments)
                .HasForeignKey(ee => ee.EquipmentId);
        }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
