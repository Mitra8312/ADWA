using Microsoft.AspNetCore.Mvc;
using ADWA.Models;

namespace ADWA.Controllers
{
    public class UserController : Controller
    {
        public readonly ActiveDirectoryService _adService;
        public UserController(ActiveDirectoryService adService)
        {
            _adService = adService;
        }
        public IActionResult DialIn()
        {
            var usersWithDialIn = _adService.GetUsersWithDialInEnabled();
            return View(usersWithDialIn);
        }
        public IActionResult Index()
        {
            var users = _adService.GetUsers();
            return View(users);
        }
    }
}
