using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Seeding;

public class TournamentSeeding : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> config)
    {
        config.HasData(
            new Tournament
            {
                Id = 1,
                Name = "City Cup – Tennis",
                Description = "Amateur tennis tournament with group stages and knockouts.",
                StartDate = new DateTime(2025, 9, 12),
                EndDate = new DateTime(2025, 9, 14),
                SportId = 2,
                IsDeleted = false
            },
            new Tournament
            {
                Id = 2,
                Name = "Basket 3-on-3 Open",
                Description = "Fast-paced 3-on-3 format, open for mixed teams.",
                StartDate = new DateTime(2025, 10, 4),
                EndDate = new DateTime(2025, 10, 5),
                SportId = 1,
                IsDeleted = false
            },
            new Tournament
            {
                Id = 3,
                Name = "Swim Sprint Challenge",
                Description = "Sprint races across age groups.",
                StartDate = new DateTime(2025, 11, 1),
                EndDate = new DateTime(2025, 11, 1),
                SportId = 3,
                IsDeleted = false
            },
            new Tournament
            {
                Id = 4,
                Name = "Winter Grand Slam 2026",
                Description = "The biggest upcoming tennis event of the year. Register now!",
                StartDate = new DateTime(2026, 12, 10),
                EndDate = new DateTime(2026, 12, 17),
                SportId = 2,
                IsDeleted = false
            }
            );
    }
}