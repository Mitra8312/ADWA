using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using ADWA.Models;

namespace ADWA.Controllers
{
    //[Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ActiveDirectoryService _adService;
        public AuthController(ActiveDirectoryService adService)
        {
            _adService = adService;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Authenticate user against Active Directory
            if (_adService.Authenticate(username, password))
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                };

                // Create ClaimsIdentity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in the user
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // Redirect to the home page or a specific URL
                return RedirectToAction("VPN", "RemoteAccess");
            }

            ViewBag.Error = "Invalid credentials";
            return RedirectToAction("Error", "RemoteAccess");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

    }
}
