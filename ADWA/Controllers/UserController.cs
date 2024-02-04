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
