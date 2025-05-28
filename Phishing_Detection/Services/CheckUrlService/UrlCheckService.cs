using Newtonsoft.Json;
using System.Text;

namespace Phishing_Detection.Services.CheckUrl
{
    public interface IUrlCheckService
    {
        Task<string> CheckUrlAsync(string url);
    }

    public class UrlCheckService(HttpClient httpClient, IConfiguration configuration) : IUrlCheckService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string? _apiKey = configuration["GoogleSafeBrowsing:ApiKey"];

        public async Task<string> CheckUrlAsync(string url)
        {
            try
            {
                var request = new
                {
                    client = new
                    {
                        clientId = "PhishingDetectionApp",
                        clientVersion = "1.0"
                    },
                    threatInfo = new
                    {
                        threatTypes = new[] { "MALWARE", "SOCIAL_ENGINEERING", "UNWANTED_SOFTWARE", "POTENTIALLY_HARMFUL_APPLICATION" },
                        platformTypes = new[] { "ANY_PLATFORM" },
                        threatEntryTypes = new[] { "URL" },
                        threatEntries = new[] { new { url } }
                    }
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var postRequest = await _httpClient.PostAsync($"https://safebrowsing.googleapis.com/v4/threatMatches:find?key={_apiKey}", content);
                var response = await postRequest.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(response) || response.Trim() == "{}")
                {
                    return "Safe";
                }
                else
                {
                    return "Suspicious";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }

}
