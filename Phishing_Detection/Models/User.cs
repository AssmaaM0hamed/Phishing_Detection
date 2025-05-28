using System.ComponentModel.DataAnnotations;

namespace Phishing_Detection.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        public List<CheckedUrl> CheckedUrls { get; set; } = new List<CheckedUrl>();
    }
}
