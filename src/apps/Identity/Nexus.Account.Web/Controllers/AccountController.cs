using Microsoft.AspNetCore.Mvc;

namespace Nexus.Account.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult FargotPassword()
        {
            return View();
        }
    }
}