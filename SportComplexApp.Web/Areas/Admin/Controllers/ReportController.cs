using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.Controllers;
using System.Text;
using System.Text.Json;

namespace SportComplexApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : BaseController
    {
        private readonly ISportService sportService;
        private readonly ISpaService spaService;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public ReportController(ISportService sportService, ISpaService spaService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.sportService = sportService;
            this.spaService = spaService;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> SportReservations(DateTime? startDate, DateTime? endDate)
        {
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            var reportData = await sportService.GetSportReservationsReportAsync(startDate, endDate);
            return View(reportData);
        }

        [HttpGet]
        public async Task<IActionResult> ExportSportReservationsCsv(DateTime? startDate, DateTime? endDate)
        {
            var reportData = await sportService.GetSportReservationsReportAsync(startDate, endDate);

            var builder = new StringBuilder();

            builder.AppendLine($"{sharedLocalizer["CsvClientName"]},{sharedLocalizer["CsvSportName"]},{sharedLocalizer["CsvDate"]},{sharedLocalizer["CsvPrice"]}");

            foreach (var item in reportData)
            {
                var safeClientName = item.ClientName?.Replace(",", " ") ?? "";
                var safeSportName = item.SportName?.Replace(",", " ") ?? "";

                if (item.IsSportDeleted)
                {
                    safeSportName += " (Deleted)";
                }

                builder.AppendLine($"{safeClientName},{safeSportName},{item.Date},{item.Price:F2}");
            }

            builder.AppendLine($",,{sharedLocalizer["CsvTotalRow"]},{reportData.Sum(x => x.Price):F2}");

            var fileBytes = Encoding.UTF8.GetBytes(builder.ToString());
            var bom = Encoding.UTF8.GetPreamble();
            var completeFile = bom.Concat(fileBytes).ToArray();

            var fileName = $"SportReservations_{DateTime.Now:yyyy-MM-dd}.csv";

            return File(completeFile, "text/csv", fileName);
        }
        [HttpGet]
        public async Task<IActionResult> SpaReservations(DateTime? startDate, DateTime? endDate)
        {
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            var reportData = await spaService.GetSpaReservationsReportAsync(startDate, endDate);
            return View(reportData);
        }

        [HttpGet]
        public async Task<IActionResult> ExportSpaReservationsCsv(DateTime? startDate, DateTime? endDate)
        {
            var reportData = await spaService.GetSpaReservationsReportAsync(startDate, endDate);

            var builder = new StringBuilder();

            builder.AppendLine($"{sharedLocalizer["CsvClientName"]},{sharedLocalizer["CsvServiceName"]},{sharedLocalizer["CsvDate"]},{sharedLocalizer["CsvPrice"]}");

            foreach (var item in reportData)
            {
                var safeClientName = item.ClientName?.Replace(",", " ") ?? "";
                var safeServiceName = item.ServiceName?.Replace(",", " ") ?? "";

                if (item.IsServiceDeleted)
                {
                    safeServiceName += " (Deleted)";
                }

                builder.AppendLine($"{safeClientName},{safeServiceName},{item.Date},{item.Price:F2}");
            }

            builder.AppendLine($",,{sharedLocalizer["CsvTotalRow"]},{reportData.Sum(x => x.Price):F2}");

            var fileBytes = Encoding.UTF8.GetBytes(builder.ToString());
            var bom = Encoding.UTF8.GetPreamble();
            var completeFile = bom.Concat(fileBytes).ToArray();

            var fileName = $"SpaReservations_{DateTime.Now:yyyy-MM-dd}.csv";

            return File(completeFile, "text/csv", fileName);
        }
    }
}