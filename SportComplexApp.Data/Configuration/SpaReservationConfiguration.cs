using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Configuration
{
    public class SpaReservationConfiguration : IEntityTypeConfiguration<SpaReservation>
    {
        public void Configure(EntityTypeBuilder<SpaReservation> builder)
        {
            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.ReservationDateTime)
                .IsRequired();

            builder.Property(sr => sr.NumberOfPeople)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(sr => sr.Client)
                .WithMany(c => c.SpaReservations)
                .HasForeignKey(sr => sr.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sr => sr.SpaService)
                .WithMany(ss => ss.SpaReservations)
                .HasForeignKey(sr => sr.SpaServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
