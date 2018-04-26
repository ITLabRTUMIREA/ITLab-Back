using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Equipments;
using Models.Events;
using Models;
using Models.DataBaseLinks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.DataBase
{
    public class DataBaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }

        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

            modelBuilder.Entity<EventUser>()
                .HasKey(t => new { t.UserId, t.EventId });

            modelBuilder.Entity<EventUser>()
                .HasOne(ee => ee.Event)
                .WithMany(ev => ev.EventUsers)
                .HasForeignKey(ee => ee.EventId);

            modelBuilder.Entity<EventUser>()
                .HasOne(ee => ee.User)
                .WithMany(eq => eq.EventUsers)
                .HasForeignKey(ee => ee.UserId);
        }
    }
}
