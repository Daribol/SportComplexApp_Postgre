using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Seeding;

public class SportTrainerSeeding : IEntityTypeConfiguration<SportTrainer>
{
    public void Configure(EntityTypeBuilder<SportTrainer> config)
    {
        config.HasData(
            new SportTrainer
            {
                SportId = 1, // Basketball
                TrainerId = 1 // Kiril Raikov
            },
            new SportTrainer
            {
                SportId = 2, // Tennis
                TrainerId = 4 // Grigor Dimitrov
            },
            new SportTrainer
            {
                SportId = 3, // Yoga
                TrainerId = 2 // Vili Markovska
            },
            new SportTrainer
            {
                SportId = 4, // CrossFit
                TrainerId = 3 // Kostadin Lefterov
            },
            new SportTrainer
            {
                SportId = 3, // Yoga
                TrainerId = 3 // Kostadin Lefterov
            }
        );
    }
}