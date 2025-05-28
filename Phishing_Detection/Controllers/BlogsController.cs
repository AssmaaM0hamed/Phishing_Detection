using Microsoft.AspNetCore.Mvc;

namespace Phishing_Detection.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Index() =>  View();
        public IActionResult FutureOfPhishingArticle() => View();
        public IActionResult MobileMalwareArticle() => View();
        public IActionResult SecurityToolsArticle() => View();
        public IActionResult TypesOfPhishingArticle() => View();
        public IActionResult PhishingArticle() => View();
        public IActionResult PhishingEmailArticle() => View();
        public IActionResult MaliciousLinkArticle() => View();
        public IActionResult AvoidingMaliciousLinksArticle() => View();


    }
}
