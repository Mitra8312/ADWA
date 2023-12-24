using Microsoft.AspNetCore.Mvc;
using ADWA.Models;
using System.Diagnostics;
using System.DirectoryServices;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ADWA.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ActiveDirectoryService _adService;
        public UserController(ActiveDirectoryService adService, IHttpContextAccessor httpContextAccessor)
        {
            _adService = adService;
            _httpContextAccessor = httpContextAccessor;
        }
        [Route("/User/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        [HttpPost]
        public IActionResult DisableDialIn([FromBody] UpdateDialIn model)
        {
            
             _adService.UpdateDialInStatus(model.SamAccountName, model.IsEnabled);
            var updatedUser = _adService.GetUserByLogin(model.SamAccountName);
            DirectoryEntry directoryEntry = (DirectoryEntry)updatedUser.GetUnderlyingObject();
            // Обновляем значение свойства msNPAllowDialin

            return Json(new
            {
                success = true,
                message = "Dial-In state update successfully",
                updatedData = new
                {
                    SamAccountName = updatedUser.SamAccountName,
                    IsDialInEnabled = directoryEntry.Properties["msNPAllowDialin"].Value
                }
            });
        }
        public IActionResult? CurrentUser ()
        {
            var windowsIdentity = _httpContextAccessor.HttpContext.User.Identity as WindowsIdentity;
            try
            {
                // Log the user's identity
                Console.WriteLine($"User: {windowsIdentity?.Name}");
                return View(windowsIdentity);


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return View("Error", ex);
            }
            
            
        }
        
    }
}
