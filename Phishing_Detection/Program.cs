using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Phishing_Detection.Data;
using Phishing_Detection.Hubs;
using Phishing_Detection.Models;
using Phishing_Detection.Repositries.CheckedUrlRepo;
using Phishing_Detection.Services.CheckUrl;
using Phishing_Detection.Services.UrlMointoring;

namespace Phishing_Detection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Repos
            builder.Services.AddHttpClient<IUrlCheckService, UrlCheckService>();
            builder.Services.AddScoped<ICheckedUrlRepository, CheckedUrlRepository>();

            //SginalR
            builder.Services.AddSignalR();
            builder.Services.AddHostedService<UrlMonitoringService>(); // Must be here

            // Add services to the container
            var connectionString = builder.Configuration.GetConnectionString("Connection") ??
                throw new InvalidOperationException("Connection string 'Connection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Configure Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // Set to false for simplicity while learning
                options.Password.RequireDigit = false; // Relax password rules for testing
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            

            builder.Services.AddControllersWithViews();



            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Add authentication BEFORE authorization
            app.UseAuthentication(); // <--- Critical: Add this!
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages(); // For Identity pages

            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}