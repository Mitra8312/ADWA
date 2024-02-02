using ADWA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Principal;

namespace ADWA.Controllers
{
	[Authorize]
	public class UserController(ActiveDirectoryService adService, IHttpContextAccessor httpContextAccessor) : Controller
	{
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly ActiveDirectoryService _adService = adService;

		[Route("/User/Error")]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult Index()
		{
			List<ApplicationUser> users = _adService.GetUsers();

			return View();
		}

		public IActionResult NewUser()
		{
			List<OrganisationUnit> organisationUnits = _adService.GetOrganisationUnits();

			return View(organisationUnits);
		}

		public IActionResult CreateUser([FromBody] ApplicationUser newUser)
		{
			try
			{
				if (_adService.CreateUser(newUser))
				{
					return Json(new
					{
						success = true,
						message = "User created successfuly"
					});
				}
				else
				{
					return Json(new
					{
						success = false,
						message = "User created unsuccessfully"
					});
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Json(new
				{
					success = true,
					message = ex.Message
				});
			}
		}

		//[HttpGet]
		//public string GetUsers()
		//{
		//    //List<ApplicationUser> users = _adService.GetUsers();

		//    List<OrganisationUnit> organisationUnits = _adService.GetOrganisationUnits();

		//    //ViewBag.OrganisationUnits = 

		//    return Newtonsoft.Json.JsonConvert.SerializeObject(organisationUnits);
		//}

		public IActionResult? CurrentUser()
		{
			WindowsIdentity? windowsIdentity = _httpContextAccessor.HttpContext.User.Identity as WindowsIdentity;
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
