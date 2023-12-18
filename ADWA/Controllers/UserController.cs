using Microsoft.AspNetCore.Mvc;
using ADWA.Models;
using System.Diagnostics;
using System.DirectoryServices;

namespace ADWA.Controllers
{
    public class UserController : Controller
    {
        public readonly ActiveDirectoryService _adService;
        public UserController(ActiveDirectoryService adService)
        {
            _adService = adService;
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
            }) ;
        }
        
    }
}
