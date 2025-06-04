using Microsoft.AspNetCore.DataProtection.Repositories;
using Phishing_Detection.Services;
using Phishing_Detection.Repositries.CheckedUrlRepo;
using Phishing_Detection.Data;
using Phishing_Detection.Services.CheckUrl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Phishing_Detection.Hubs;
using Phishing_Detection.Models;


namespace Phishing_Detection.Services.UrlMointoring
{
    public class UrlMonitoringService : BackgroundService, IUrlMonitoringService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;

        public UrlMonitoringService(IServiceProvider serviceProvider, IHubContext<NotificationHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await MonitorUrlsAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); 
            }
        }
        public async Task MonitorUrlsAsync()
        {
            Console.WriteLine("Monitoring URLs...");
            using var scope = _serviceProvider.CreateScope();
            var checkedUrlRepo = scope.ServiceProvider.GetRequiredService<ICheckedUrlRepository>();
            var phishingService = scope.ServiceProvider.GetRequiredService<IUrlCheckService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var urlsToMonitor = await checkedUrlRepo.GetCheckedUrlsAsync();

            foreach (var checkedUrl in urlsToMonitor)
            {
                string newStatus = await phishingService.CheckUrlAsync(checkedUrl.Url);

                if (checkedUrl.Status != newStatus)
                {
                    await checkedUrlRepo.UpdateUrlStatusAsync(checkedUrl.Url, newStatus);

                    var usersWhoCheckedUrl = await userManager.Users
                        .Where(u => u.CheckedUrls.Any(cu => cu.Url == checkedUrl.Url))
                        .ToListAsync();

                    var message = $"The URL '{checkedUrl.Url}' has changed from '{checkedUrl.Status}' to '{newStatus}'.";
                    foreach (var user in usersWhoCheckedUrl)
                    {
                        await _hubContext.Clients.User(user.Id).SendAsync("ReceiveNotification", message);
                    }
                }
            }
        }
    }

    
}
