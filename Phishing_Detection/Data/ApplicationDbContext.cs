using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Phishing_Detection.Models;

namespace Phishing_Detection.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CheckedUrl> CheckedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the relationship between ApplicationUser and CheckedUrl
            builder.Entity<CheckedUrl>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CheckedUrls)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Deletes URLs if user is deleted
        }
    }
}
