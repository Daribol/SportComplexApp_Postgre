using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Seeding;

public class TrainerSeeding : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> config)
    {
        config.HasData(
            new Trainer
            {
                Id = 1,
                Name = "Kiril",
                LastName = "Raikov",
                Bio = "Certified basketball coach with 8+ years of experience.",
                ImageUrl = "/images/kiril_raikov.jpg",
                IsDeleted = false,
                ClientId = null
            },
            new Trainer
            {
                Id = 2,
                Name = "Vili",
                LastName = "Markovska",
                Bio = "Yoga & Pilates instructor focused on mobility and mindfulness.",
                ImageUrl = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=500&h=500&fit=crop",
                IsDeleted = false,
                ClientId = null
            },
            new Trainer
            {
                Id = 3,
                Name = "Kostadin",
                LastName = "Lefterov",
                Bio = "CrossFit coach, competition-level conditioning specialist.",
                ImageUrl = "/images/koceto.jpg",
                ClientId = null
            },
            new Trainer
            {
                Id = 4,
                Name = "Grigor",
                LastName = "Dimitrov",
                Bio = "Tennis training: technique, tactics, and matchplay.",
                ImageUrl = "/images/grisho.jpg",
                IsDeleted = false,
                ClientId = null
            }
        );
    }
}