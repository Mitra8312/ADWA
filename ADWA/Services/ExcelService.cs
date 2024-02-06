using ADWA.DataBase;
using ADWA.Models;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace ADWA.Services
{
	public class ExcelService
	{
		private static readonly DBContext _context = new();

		/// <summary>
		/// Метод обработки файла для подключения и отключения удаленного доступа пользователям с добавлениям их в БД
		/// </summary>
		/// <param name="path">Путь к обрабатываемому файлу</param>
		/// <param name="_adService">Экземпляр класса для доступа к AD</param>
		/// <returns></returns>
		public static async Task<bool> ReadFile(string path, ActiveDirectoryService _adService)
		{

			try
			{
				List<AddRA> usersToDisconnect = [];
				List<AddRA> usersToConnect = [];

				List<ApplicationUser> usersInDatabase = _context.Users.ToList();
				List<ApplicationUser> usersInAD = _adService.GetUsers();

				ExcelPackage.LicenseContext = new OfficeOpenXml.LicenseContext?(LicenseContext.NonCommercial);
				ExcelWorksheet worksheet;

				ExcelPackage package = new(new FileInfo(path));
				worksheet = package.Workbook.Worksheets[0];

				int rowCount = 0;
				int columns = 0;

				if (worksheet.Dimension != null)
				{
					rowCount = worksheet.Dimension.Rows;
					columns = worksheet.Dimension.Columns;
				}
                else
                {
					return false;
                }

                try
				{
					for (int i = 2; i <= rowCount; i++)
					{
						if (DateTime.TryParse(worksheet.Cells[i, 3].Text, out DateTime dateValue))
						{
							if (dateValue < DateTime.Now)
							{
								usersToDisconnect.Add(new AddRA(worksheet.Cells[i, 2].Text, dateValue.ToString()));
							}
							else
							{
								usersToConnect.Add(new AddRA(worksheet.Cells[i, 2].Text, dateValue.ToString()));
							}
						}
						else
						{
							usersToConnect.Add(new AddRA(worksheet.Cells[i, 2].Text, "Производственная необходимость"));
						}
					}

					if (usersToDisconnect.Count > 0)
					{
						await ProcessDisconnectUsers(usersInDatabase, usersToDisconnect, _adService);
					}

					if (usersToConnect.Count > 0)
					{
						await ProcessConnectUsers(usersInDatabase, usersInAD, usersToConnect, _adService);
					}

					return true;
				}
				catch
				{
					return false;
				}
				finally
				{
					package.Save();
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Отключение удаленного доступа пользователям
		/// </summary>
		/// <param name="usersInDatabase">Пользователи в базе данных</param>
		/// <param name="usersToDisconnect">Пользователи для отключения удаленного доступа</param>
		/// <param name="adService">Экземпляр класса для доступа к AD</param>
		/// <returns></returns>
		private static async Task ProcessDisconnectUsers(List<ApplicationUser> usersInDatabase, List<AddRA> usersToDisconnect, ActiveDirectoryService adService)
		{
			foreach (var usToDisconnect in usersToDisconnect)
			{
				var userInDB = usersInDatabase.FirstOrDefault(u => u.GetSamAccountName() == usToDisconnect.selectUser);
				if (userInDB != null)
				{
					adService.UpdateDialInStatus(usToDisconnect.selectUser, false);
					_ = _context.Users.Remove(userInDB);
					await _context.SaveChangesAsync();
				}
			}
		}

		/// <summary>
		/// Подключение удаленного дуступа пользователям
		/// </summary>
		/// <param name="usersInDatabase">Пользователи в базе данных</param>
		/// <param name="usersInAD">Пользоватлеи в Active Directory</param>
		/// <param name="usersToConnect">Пользователи для подключения</param>
		/// <param name="adService">Экземпляр класса для доступа к AD</param>
		/// <returns></returns>
		private static async Task ProcessConnectUsers(List<ApplicationUser> usersInDatabase, List<ApplicationUser> usersInAD, List<AddRA> usersToConnect, ActiveDirectoryService adService)
		{
			foreach (var usToConnect in usersToConnect)
			{
				var userInAD = usersInAD.FirstOrDefault(u => u.GetSamAccountName() == usToConnect.selectUser);
				var userInDB = usersInDatabase.FirstOrDefault(u => u.GetSamAccountName() == usToConnect.selectUser);

				if (userInAD != null && userInDB == null)
				{
					adService.UpdateDialInStatus(usToConnect.selectUser, true);


					if (Regex.IsMatch(usToConnect.DateOfDisconnect, "[A-Za-zА-Яа-я]"))
					{
						userInAD.SetDateOfDisconnect("Производственная необходимость");
					}
					else
					{
						userInAD.SetDateOfDisconnect(usToConnect.DateOfDisconnect);
					}


					userInAD.SetDateOfDisconnect(usToConnect.DateOfDisconnect);
					userInAD.SetIsDialInEnabled(true);

					await _context.Users.AddAsync(userInAD);
					await _context.SaveChangesAsync();
				}
				else if (userInDB != null)
				{
					userInDB.SetDateOfDisconnect(usToConnect.DateOfDisconnect);
					await _context.SaveChangesAsync();
				}
			}
		}
	}
}
