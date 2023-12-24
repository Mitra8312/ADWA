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
        // Process login
    [HttpPost]
    public IActionResult Login(string username, string password)
        {
            // Authenticate user against Active Directory
            if (_adService.Authenticate(username, password))
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    // Add other claims as needed
                };

                // Create ClaimsIdentity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in the user
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // Redirect to the home page or a specific URL
                return RedirectToAction("Index", "User");
            }


            // Authentication failed
            ViewBag.Error = "Invalid credentials";
            return RedirectToAction("Error","Home");
        }
        // Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
        
    }
}
