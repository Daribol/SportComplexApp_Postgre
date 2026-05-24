using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Configuration
{
    public class SportTrainerConfiguration : IEntityTypeConfiguration<SportTrainer>
    {
        public void Configure(EntityTypeBuilder<SportTrainer> builder)
        {
            builder.HasKey(st => new { st.SportId, st.TrainerId });

            builder.HasOne(st => st.Sport)
                .WithMany(s => s.SportTrainers)
                .HasForeignKey(st => st.SportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(st => st.Trainer)
                .WithMany(t => t.SportTrainers)
                .HasForeignKey(st => st.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
