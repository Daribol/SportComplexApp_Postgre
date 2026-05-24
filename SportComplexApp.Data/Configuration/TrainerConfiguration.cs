using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;
using static SportComplexApp.Common.EntityValidationConstants.Trainer;

namespace SportComplexApp.Data.Configuration
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder.Property(t => t.Bio)
                .IsRequired(false)
                .HasMaxLength(BioMaxLenght)
                .HasDefaultValue(null);

            builder.Property(t => t.ImageUrl)
                .IsRequired(false)
                .HasMaxLength(ImageUrlMaxLength)
                .HasDefaultValue(null);

            builder.Property(t => t.ClientId)
                .IsRequired(false);

            builder.HasMany(t => t.TrainerSessions)
                .WithOne(ts => ts.Trainer)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.SportTrainers)
                .WithOne(st => st.Trainer)
                .HasForeignKey(st => st.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Reservations)
                .WithOne(r => r.Trainer)
                .HasForeignKey(r => r.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
