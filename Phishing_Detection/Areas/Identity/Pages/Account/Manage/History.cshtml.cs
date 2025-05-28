using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Phishing_Detection.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phishing_Detection.Areas.Identity.Pages.Account.Manage
{
    public class HistoryModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HistoryModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<CheckedUrl> CheckedUrls { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.Users
                .Include(u => u.CheckedUrls) // Include the CheckedUrls collection
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CheckedUrls = user.CheckedUrls.ToList();
        }
    }
}