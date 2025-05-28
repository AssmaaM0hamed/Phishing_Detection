using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Phishing_Detection.Repositries.CheckedUrlRepo;
using Phishing_Detection.Services.CheckUrl;

namespace Phishing_Detection_App.Controllers
{
    [Authorize]
    public class CheckUrlController : Controller
    {
        private readonly IUrlCheckService _phishingService;
        private readonly ICheckedUrlRepository _checkedUrlRepo;

        public CheckUrlController(IUrlCheckService phishingService, ICheckedUrlRepository checkedUrlRepo)
        {
            _phishingService = phishingService;
            _checkedUrlRepo = checkedUrlRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckUrl(string url)
        {
            if (!IsValidUrl(url))
            {
                ViewBag.Result = "Invalid URL!";
                ViewBag.StorageStatus = "Not stored due to invalid URL.";
                return View("Index");
            }

            var result = await _phishingService.CheckUrlAsync(url);
            var storeResult = await _checkedUrlRepo.StoreUrl(url, result, User);

            if (storeResult is not OkResult)
            {
                if (storeResult is UnauthorizedResult)
                {
                    ViewBag.Result = "User not authenticated.";
                    ViewBag.StorageStatus = "Not stored: Authentication required.";
                }
                else if (storeResult is BadRequestObjectResult badRequest)
                {
                    ViewBag.Result = "Error saving URL check: " + string.Join(", ", (badRequest.Value as IEnumerable<IdentityError>)?.Select(e => e.Description));
                    ViewBag.StorageStatus = "Failed to store in database.";
                }
                else
                {
                    ViewBag.Result = "Unknown error.";
                    ViewBag.StorageStatus = "Storage status unknown.";
                }
                return View("Index");
            }

            ViewBag.Result = result;
            ViewBag.StorageStatus = "Stored successfully in database.";
            return View("Index");
        }
        private bool IsValidUrl(string url)
        {
            string pattern = @"^(https?:\/\/)?([\w.-]+)\.([a-z]{2,6})([/\w .-]*)*\/?$";
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }
    }
}
