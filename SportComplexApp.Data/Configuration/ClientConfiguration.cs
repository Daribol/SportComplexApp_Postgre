using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;
using static SportComplexApp.Common.EntityValidationConstants.Client;

namespace SportComplexApp.Data.Configuration
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(FirstNameMaxLength);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(LastNameMaxLength);
        }
    }
}
