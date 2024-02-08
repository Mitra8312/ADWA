using ADWA.DataBase;
using ADWA.Models;
using ADWA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;

namespace ADWA.Controllers
{
	[Authorize]
	public class RemoteAccessController(ActiveDirectoryService adService) : Controller
	{
		private readonly ActiveDirectoryService _adService = adService;
		private readonly DBContext _dbContext = new();
		public IActionResult VPN()
		{
			return View();
		}

		public IActionResult HandRemoteAccess()
		{
			List<ApplicationUser> users = _adService.GetUsers();

			return View(users);
		}

		public async Task<IActionResult> AutomaticRemoteAccess()
		{
			AutomaticRemoteAccessModel model = new AutomaticRemoteAccessModel();

			model.UsersWithRA = _dbContext.Users.ToList();
			model.UsersWithoutRA = _adService.GetUsersWithoutRemoteAccess();

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AddUserToRA([FromBody] AddRA user)
		{
			try
			{
				ApplicationUser userToAdd = new();

				List<ApplicationUser> users = _adService.GetUsersWithoutRemoteAccess();

				foreach (var i in users)
				{
					if (i.GetSamAccountName() == user.selectUser)
					{
						userToAdd = i;

						_adService.UpdateDialInStatus(user.selectUser, true);

						break;
					}
				}

				userToAdd.SetIsDialInEnabled(true);
				userToAdd.SetDateOfDisconnect(user.DateOfDisconnect);

				await _dbContext.Users.AddAsync(userToAdd);
				await _dbContext.SaveChangesAsync();

				
				return Json(new
				{
					success = true
				});

			}
			catch
			{
				return Json(new
				{
					success = false
				});
			}
		}

		public async Task<IActionResult> DelUserToRA(string SamAccountName)
		{
			try
			{
				ApplicationUser userToDis = new();
				foreach (var i in _dbContext.Users.ToList())
				{
					if (i.GetSamAccountName() == SamAccountName)
					{
						userToDis = i;

						_adService.UpdateDialInStatus(SamAccountName, false);

						break;
					}
				}

				_dbContext.Users.Remove(userToDis);

				await _dbContext.SaveChangesAsync();

				return RedirectToAction("AutomaticRemoteAccess");

			}
			catch
			{
				return Json(new
				{
					success = false
				});
			}
		}

		[HttpPost]
		public async Task<IActionResult> DisableDialIn([FromBody] UpdateDialIn model)
		{
			_adService.UpdateDialInStatus(model.SamAccountName, model.IsEnabled);

			if (model.IsEnabled)
			{
				ApplicationUser user = new();
				foreach (var u in _adService.GetUsers())
				{
					if (u.GetSamAccountName() == model.SamAccountName)
					{
						user = u;
						break;
					}
				}

				user.SetIsDialInEnabled(model.IsEnabled);

				user.SetDateOfDisconnect("Производственная необходимость");

				await _dbContext.Users.AddAsync(user);

				await _dbContext.SaveChangesAsync();
			}
			else
			{
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.SamAccountName == model.SamAccountName);

				_dbContext.Remove(user);

				await _dbContext.SaveChangesAsync();
			}

			System.DirectoryServices.AccountManagement.UserPrincipal updatedUser = _adService.GetUserByLogin(model.SamAccountName);

			DirectoryEntry directoryEntry = (DirectoryEntry)updatedUser.GetUnderlyingObject();
			// Обновляем значение свойства msNPAllowDialin

			return Json(new
			{
				success = true,
				message = "Dial-In state update successfully",
				updatedData = new
				{
					updatedUser.SamAccountName,
					IsDialInEnabled = directoryEntry.Properties["msNPAllowDialin"].Value
				}
			});
		}

		[HttpPost]
		public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
		{
			try
			{
				if (file != null && file.Length > 0)
				{
					var fileName = DateTime.Now.ToString().Replace(" ", "").Replace(".", "").Replace(":", "") + "_" + file.FileName;
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await file.CopyToAsync(stream);
					}

					if (await ExcelService.ReadFile(filePath, _adService))
					{
						return RedirectToAction("AutomaticRemoteAccess");
					}
					else
					{
						return Json(new { success = false, message = "Файл был обработан с ошибкой" });
					}
				}

				return Json(new { success = false, message = "Файл не был получен" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}

		public async Task<IActionResult> SaveChangesRemoteAccess([FromBody] AddRA user)
		{
			try
			{
				ApplicationUser appUser = await _dbContext.Users.FirstAsync(u => u.GetSamAccountName().Equals(user.SelectUser));

				appUser.SetDateOfDisconnect(user.DateOfDisconnect);

				_dbContext.SaveChanges();

				return Json(new
				{
					success = true,
				});
			}
			catch
			{
				return Json(new
				{
					success = false,
				});
			}
		}
  
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
