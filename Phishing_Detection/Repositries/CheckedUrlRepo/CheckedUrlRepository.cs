using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Phishing_Detection.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Phishing_Detection.Data;
using Microsoft.EntityFrameworkCore;

namespace Phishing_Detection.Repositries.CheckedUrlRepo
{
    public class CheckedUrlRepository : ICheckedUrlRepository
    {
        //private readonly HttpClient _httpClient;
        //private readonly string _apiKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CheckedUrlRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            //_httpClient = httpClient;
            //_apiKey = configuration["GoogleSafeBrowsing:ApiKey"];
            _userManager = userManager;
            _context = context;
        }

        //public async Task<string> CheckUrlAsync(string url)
        //{
        //    try
        //    {
        //        var request = new
        //        {
        //            client = new
        //            {
        //                clientId = "PhishingDetectionApp",
        //                clientVersion = "1.0"
        //            },
        //            threatInfo = new
        //            {
        //                threatTypes = new[] { "MALWARE", "SOCIAL_ENGINEERING", "UNWANTED_SOFTWARE", "POTENTIALLY_HARMFUL_APPLICATION" },
        //                platformTypes = new[] { "ANY_PLATFORM" },
        //                threatEntryTypes = new[] { "URL" },
        //                threatEntries = new[] { new { url } }
        //            }
        //        };

        //        var json = JsonConvert.SerializeObject(request);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var postRequest = await _httpClient.PostAsync($"https://safebrowsing.googleapis.com/v4/threatMatches:find?key={_apiKey}", content);
        //        var response = await postRequest.Content.ReadAsStringAsync();

        //        if (string.IsNullOrEmpty(response) || response.Trim() == "{}")
        //        {
        //            return "Safe";
        //        }
        //        else
        //        {
        //            return "Suspicious";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error: {ex.Message}";
        //    }
        //}

        public async Task<IActionResult> StoreUrl(string url, string result, ClaimsPrincipal claimsUser)
        {
            var user = await _userManager.GetUserAsync(claimsUser);
            if (user == null)
            {
                return new UnauthorizedResult();
            }

            var checkedUrl = new CheckedUrl
            {
                Url = url,
                Status = result.ToLower().Contains("suspicious") ? "Suspicious" : "Safe",
                CheckedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            _context.CheckedUrls.Add(checkedUrl);
            user.CheckedUrls.Add(checkedUrl);
            user.CheckCount++;
            await _context.SaveChangesAsync();

            return new OkResult();
        }
        public async Task<IEnumerable<CheckedUrl>> GetCheckedUrlsAsync()
        {
            var users = await _userManager.Users.Include(u => u.CheckedUrls).ToListAsync();

            return users.SelectMany(u => u.CheckedUrls) ?? new List<CheckedUrl>();
        }

        public async Task<bool> UpdateUrlStatusAsync(string url, string newStatus)
        {
            var checkedUrl = await _context.CheckedUrls.FirstOrDefaultAsync(c => c.Url == url);

            if (checkedUrl == null)
                return false;

            string updatedStatus = newStatus.ToLower().Contains("suspicious") ? "Suspicious" : "Safe";

            if (checkedUrl.Status != updatedStatus)
            {
                checkedUrl.Status = updatedStatus;
                await _context.SaveChangesAsync();
            }

            return true;
        }

    }
}
