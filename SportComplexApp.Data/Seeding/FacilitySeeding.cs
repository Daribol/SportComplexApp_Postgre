using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Seeding;

public class FacilitySeeding : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> config)
    {
        config.HasData(
            new Facility
            {
                Id = 1,
                Name = "Indoor Arena",
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSASkhPWQ1zqXLUUBYFlMhZn4yMaeMu8yVjZg&s",
                IsDeleted = false
            },
            new Facility
            {
                Id = 2,
                Name = "Tennis Center",
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSwhAHcH2_eq_owatwVzZDK2JyB4QcdFPya9g&s",
                IsDeleted = false
            },
            new Facility
            {
                Id = 3,
                Name = "Fitness Studio",
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQpZB-L2ZhjG6LXt6ByYv4vOYWu3lGvTvKNOQ&s",
                IsDeleted = false
            }
        );
    }
}