using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;
using static SportComplexApp.Common.EntityValidationConstants.SpaService;

namespace SportComplexApp.Data.Configuration
{
    public class SpaServiceConfiguration : IEntityTypeConfiguration<SpaService>
    {
        public void Configure(EntityTypeBuilder<SpaService> builder)
        {
            builder.HasKey(ss => ss.Id);

            builder.Property(ss => ss.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder.Property(ss => ss.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(ss => ss.ProcedureDetails)
                .IsRequired();

            builder.Property(ss => ss.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasPrecision(10, 2);

            builder.Property(ss => ss.ImageUrl)
                .HasMaxLength(ImageUrlMaxLength);

            builder.Property(ss => ss.Duration)
                .IsRequired()
                .HasDefaultValue(60);
        }
    }
}
