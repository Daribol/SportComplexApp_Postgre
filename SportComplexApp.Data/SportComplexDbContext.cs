using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportComplexApp.Data.Models;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SportComplexApp.Data
{
    public class SportComplexDbContext : IdentityDbContext<Client>
    {
        public SportComplexDbContext()
        {

        }
        public SportComplexDbContext(DbContextOptions options)
            : base(options)
        {

        }
       
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Sport> Sports { get; set; } = null!;
        public virtual DbSet<Facility> Facilities { get; set; } = null!;
        public virtual DbSet<Trainer> Trainers { get; set; } = null!;
        public virtual DbSet<SportTrainer> SportTrainers { get; set; } = null!;
        public virtual DbSet<SpaReservation> SpaReservations { get; set; } = null!;
        public virtual DbSet<TournamentRegistration> TournamentRegistrations { get; set; } = null!;
        public virtual DbSet<SpaService> SpaServices { get; set; } = null!;
        public virtual DbSet<Tournament> Tournaments { get; set; } = null!;
        public virtual DbSet<TrainerSession> TrainerSessions { get; set; } = null!;
        public virtual DbSet<Log_22180008> Logs_22180008 { get; set; } = null!;

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                var property = entityEntry.Entity.GetType().GetProperty("LastModified_22180008");

                if (property != null)
                {
                    property.SetValue(entityEntry.Entity, DateTime.UtcNow);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("22180008");

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var property = entityType.ClrType.GetProperty("LastModified_22180008");
                if (property != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("LastModified_22180008")
                        .IsConcurrencyToken()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (!string.IsNullOrEmpty(tableName) && tableName != "Logs_22180008")
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .ToTable(tb => tb.HasTrigger($"trg_{tableName}_Audit"));
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
