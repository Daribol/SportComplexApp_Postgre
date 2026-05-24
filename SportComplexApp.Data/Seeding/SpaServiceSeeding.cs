using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Seeding;

public class SpaServiceSeeding : IEntityTypeConfiguration<SpaService>
{
    public void Configure(EntityTypeBuilder<SpaService> config)
    {
        config.HasData(
            new SpaService
            {
                Id = 1,
                Name = "Classic Full-Body Massage",
                Description = "Relaxing full-body massage with essential oils.",
                ProcedureDetails = "Mild to medium pressure, lavender-almond oil.",
                Price = 55.00m,
                ImageUrl = "https://images.unsplash.com/photo-1620050382792-434b5828873d?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NXx8Q2xhc3NpYyUyMEZ1bGwlMjBCb2R5JTIwTWFzc2FnZXxlbnwwfDB8MHx8fDI%3D",
                Duration = 60,
                IsDeleted = false
            },
            new SpaService
            {
                Id = 2,
                Name = "Hot Stone Therapy",
                Description = "Warm volcanic stones to soothe deep muscle tension.",
                ProcedureDetails = "Progressive heating and placement along energy meridians.",
                Price = 75.00m,
                ImageUrl = "https://images.unsplash.com/photo-1610402601271-5b4bd5b3eba4?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8SG90JTIwU3RvbmUlMjBUaGVyYXB5fGVufDB8MHwwfHx8Mg%3D%3D",
                Duration = 70,
                IsDeleted = false
            },
            new SpaService
            {
                Id = 3,
                Name = "Sauna + Steam Room Package",
                Description = "Combined access to sauna and steam room.",
                ProcedureDetails = "30 min sauna + 20 min steam, plus hydration.",
                Price = 25.00m,
                ImageUrl = "https://images.unsplash.com/photo-1712659604528-b179a3634560?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8N3x8U2F1bmF8ZW58MHwwfDB8fHwy",
                Duration = 60,
                IsDeleted = false
            }
        );
    }
}