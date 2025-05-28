using Microsoft.AspNetCore.Mvc;
using Phishing_Detection.Models;
using System.Security.Claims;

namespace Phishing_Detection.Repositries.CheckedUrlRepo
{
    public interface ICheckedUrlRepository
    {
        //public Task<string> CheckUrlAsync(string url);
        public Task<IActionResult> StoreUrl(string url, string result, ClaimsPrincipal claimsUser);
        public Task<IEnumerable<CheckedUrl>> GetCheckedUrlsAsync();
        public Task<bool> UpdateUrlStatusAsync(string url, string newStatus);

    }
}
