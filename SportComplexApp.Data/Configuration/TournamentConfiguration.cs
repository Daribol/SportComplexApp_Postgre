using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;
using static SportComplexApp.Common.EntityValidationConstants.Tournament;

namespace SportComplexApp.Data.Configuration
{
    public class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(t => t.StartDate)
                .IsRequired();

            builder.Property(t => t.EndDate)
                .IsRequired();
        }
    }
}
