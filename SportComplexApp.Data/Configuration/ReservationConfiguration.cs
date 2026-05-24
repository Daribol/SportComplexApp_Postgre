using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Configuration
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReservationDateTime)
                .IsRequired();

            builder.Property(r => r.NumberOfPeople)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(r => r.Client)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Sport)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Trainer)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
