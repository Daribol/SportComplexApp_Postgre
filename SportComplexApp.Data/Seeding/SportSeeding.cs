using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;
using static System.Net.WebRequestMethods;

namespace SportComplexApp.Data.Seeding;

public class SportSeeding : IEntityTypeConfiguration<Sport>
{
    public void Configure(EntityTypeBuilder<Sport> config)
    {
        config.HasData(
            new Sport
            {
                Id = 1,
                Name = "Basketball",
                FacilityId = 1,
                Price = 30.00m,
                ImageUrl = "https://images.unsplash.com/photo-1519766304817-4f37bda74a26?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTF8fGJhc2tldGJhbGx8ZW58MHwwfDB8fHwy",
                Duration = 60,
                MinPeople = 2,
                MaxPeople = 10,
                IsDeleted = false
            },
            new Sport
            {
                Id = 2,
                Name = "Tennis",
                FacilityId = 2,
                Price = 25.00m,
                ImageUrl = "https://images.unsplash.com/flagged/photo-1576972405668-2d020a01cbfa?q=80&w=1174&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Duration = 60,
                MinPeople = 2,
                MaxPeople = 4,
                IsDeleted = false
            },
            new Sport
            {
                Id = 3,
                Name = "Yoga (group session)",
                FacilityId = 3,
                Price = 10.00m,
                ImageUrl = "https://images.unsplash.com/photo-1588286840104-8957b019727f?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OXx8eW9nYXxlbnwwfDB8MHx8fDI%3D",
                Duration = 60,
                MinPeople = 4,
                MaxPeople = 20,
                IsDeleted = false
            },
            new Sport
            {
                Id = 4,
                Name = "CrossFit (group session)",
                FacilityId = 3,
                Price = 15.00m,
                ImageUrl = "https://images.unsplash.com/photo-1547226238-e53e98a8e59d?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTc3fHxjcm9zc0ZpdHxlbnwwfDB8MHx8fDI%3D",
                Duration = 50,
                MinPeople = 4,
                MaxPeople = 16,
                IsDeleted = false
            },
            new Sport
            {
                Id = 5,
                Name = "Table tennis",
                FacilityId = 1,
                Price = 10.00m,
                ImageUrl = "https://images.unsplash.com/photo-1659303388053-6078a001ea21?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MzV8fHRhYmxlJTIwdGVubmlzfGVufDB8MHwwfHx8Mg%3D%3D",
                Duration = 60,
                MinPeople = 2,
                MaxPeople = 4,
                IsDeleted = false
            });
    }
}