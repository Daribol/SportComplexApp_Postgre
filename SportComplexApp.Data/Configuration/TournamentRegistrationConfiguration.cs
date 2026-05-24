using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Configuration
{
    public class TournamentRegistrationConfiguration : IEntityTypeConfiguration<TournamentRegistration>
    {
        public void Configure(EntityTypeBuilder<TournamentRegistration> builder)
        {
            builder.HasKey(tr => tr.Id);

            builder.HasOne(tr => tr.Client)
                .WithMany(c => c.TournamentRegistrations)
                .HasForeignKey(tr => tr.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tr => tr.Tournament)
                .WithMany(t => t.TournamentRegistrations)
                .HasForeignKey(tr => tr.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
