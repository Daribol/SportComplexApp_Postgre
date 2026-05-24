using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.ViewModels.Home;

namespace SportComplexApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ISportService sportService;
    private readonly ITrainerService trainerService;
    private readonly ISpaService spaService;
    private readonly IMemoryCache memoryCache;
    public HomeController(ISportService sportService, ITrainerService trainerService, ISpaService spaService, IMemoryCache memoryCache)
    {
        this.sportService = sportService;
        this.trainerService = trainerService;
        this.spaService = spaService;
        this.memoryCache = memoryCache;
    }

    public async Task<IActionResult> Index()
    {
        const string homePageCacheKey = "HomePageDataCache";

        if (!memoryCache.TryGetValue(homePageCacheKey, out HomePageViewModel model))
        {
            model = new HomePageViewModel
            {
                Sports = await sportService.GetAllForHomeAsync(),
                Trainers = await trainerService.GetAllForHomeAsync(),
                SpaProcedures = await spaService.GetAllForHomeAsync()
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            memoryCache.Set(homePageCacheKey, model, cacheOptions);
        }

        return View(model);
    }

    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode == 404)
        {
            return View("Error404");
        }

        return View("Error500");
    }
}
