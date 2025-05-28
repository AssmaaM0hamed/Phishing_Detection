using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Phishing_Detection.Data.Migrations;

namespace Phishing_Detection.Models
{
    public class CheckedUrl
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }

        [Required]
        public string Status { get; set; } 

        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
