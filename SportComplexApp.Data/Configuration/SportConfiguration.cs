using Microsoft.EntityFrameworkCore;
using SportComplexApp.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static SportComplexApp.Common.EntityValidationConstants.Sport;

namespace SportComplexApp.Data.Configuration
{
    public class SportConfiguration : IEntityTypeConfiguration<Sport>
    {
        public void Configure(EntityTypeBuilder<Sport> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder.Property(s => s.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasPrecision(10, 2);

            builder.Property(s => s.ImageUrl)
                .IsRequired(false)
                .HasMaxLength(ImageUrlMaxLength)
                .HasDefaultValue(null);

            builder.Property(s => s.Duration)
                .IsRequired();

            builder.Property(s => s.MinPeople)
                .IsRequired();

            builder.Property(s => s.MaxPeople)
                .IsRequired();

            builder.HasOne(s => s.Facility)
                .WithMany(f => f.Sports)
                .HasForeignKey(s => s.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
