using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Configuration
{
    public class TrainerSessionConfiguration : IEntityTypeConfiguration<TrainerSession>
    {
        public void Configure(EntityTypeBuilder<TrainerSession> builder)
        {
            builder.HasKey(ts => ts.Id);

            builder.Property(ts => ts.StartTime)
                .IsRequired();

            builder.Property(ts => ts.EndTime)
                .IsRequired();

            builder.HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerSessions)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
