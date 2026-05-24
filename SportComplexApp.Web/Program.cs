using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Services;
using SportComplexApp.Services.Mapping;
using SportComplexApp.Web.ViewModels;
using SportComplexApp.Services.Data;
using SportComplexApp.Data.Configuration;
using SportComplexApp.Common;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("SQLServer");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add services to the container.
builder.Services
    .AddDbContext<SportComplexDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<Client, IdentityRole>(option =>
    {
        option.SignIn.RequireConfirmedAccount = false;
        option.Password.RequireDigit = true;
        option.Password.RequiredLength = 6;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequireUppercase = true;
        option.Password.RequireLowercase = true;
        option.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<SportComplexDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(cfg =>
cfg.LoginPath = "/Identity/Account/Login");

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

builder.Services.AddMemoryCache();

builder.Services.AddRazorPages();

builder.Services.AddSingleton<TranslationService>();
builder.Services.AddScoped<ISportService, SportService>();
builder.Services.AddScoped<ISpaService, SportComplexApp.Services.Data.SpaService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(cfg =>
{
    cfg.Filters.Add<GlobalTranslationFilter>();
})
.AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(SharedResource));
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "bg" };
    options.SetDefaultCulture("en");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});

var app = builder.Build();

app.UseRequestLocalization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DatabaseSeeder.SeedAllAsync(services);
}

AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Errors",
    pattern: "{controller=Home}/{action=Index}/{statusCode?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
