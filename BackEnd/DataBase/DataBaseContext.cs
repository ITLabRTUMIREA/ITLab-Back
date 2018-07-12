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
using Models.People;

namespace BackEnd.DataBase
{
    public class DataBaseContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }

        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlaceEquipment>()
                .HasKey(t => new { t.EquipmentId, t.PlaceId });

            modelBuilder.Entity<PlaceEquipment>()
                .HasOne(pe => pe.Place)
                .WithMany(pl => pl.PlaceEquipments)
                .HasForeignKey(pl => pl.PlaceId);

            modelBuilder.Entity<PlaceEquipment>()
                .HasOne(pe => pe.Equipment)
                .WithMany(eq => eq.PlaceEquipments)
                .HasForeignKey(pe => pe.EquipmentId);


            modelBuilder.Entity<PlaceUserRole>()
                .HasKey(t => new { t.UserId, t.PlaceId });

            modelBuilder.Entity<PlaceUserRole>()
                .HasOne(pur => pur.Place)
                .WithMany(ev => ev.PlaceUserRoles)
                .HasForeignKey(pur => pur.PlaceId);

            modelBuilder.Entity<PlaceUserRole>()
                .HasOne(pur => pur.User)
                .WithMany(u => u.PlaceUserRoles)
                .HasForeignKey(pur => pur.UserId);

            modelBuilder.Entity<PlaceUserRole>()
                .HasOne(pur => pur.Role)
                .WithMany(r => r.PlaceUserRoles)
                .HasForeignKey(pur => pur.RoleId);
        }
    }
}
