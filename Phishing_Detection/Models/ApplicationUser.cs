using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Phishing_Detection.Models;

namespace Phishing_Detection.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public int CheckCount { get; set; }
        public DateTime? LastCheckDate { get; set; }
        public bool ReceiveNotifications { get; set; } = false;

        public ICollection<CheckedUrl> CheckedUrls { get; set; } = new List<CheckedUrl>();

    }
}
