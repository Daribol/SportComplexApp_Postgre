using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Services.Data;
using SportComplexApp.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SportComplexApp.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string? connectionString = builder.Configuration.GetConnectionString("SQLServer");

            // Add services to the container.
            builder.Services
                .AddDbContext<SportComplexDbContext>(options =>
                    options.UseSqlServer(connectionString));

            builder.Services.AddAuthorization();
            builder.Services
                .AddIdentityApiEndpoints<Client>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<SportComplexDbContext>();

            builder.Services.AddScoped<ISportService, SportService>();
            builder.Services.AddScoped<ISpaService, SportComplexApp.Services.Data.SpaService>();
            builder.Services.AddScoped<ITournamentService, TournamentService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFacilityService, FacilityService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.WithOrigins("https://localhost:7226")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapIdentityApi<Client>();

            app.MapControllers();

            app.Run();
        }
    }
}
