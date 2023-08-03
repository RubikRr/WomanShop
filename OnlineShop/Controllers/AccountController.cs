using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;

namespace WomanShop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        public AccountController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index(string userName)
        {
            var user = userManager.FindByNameAsync(userName).Result;
            return View(user);
        }
    }
}
