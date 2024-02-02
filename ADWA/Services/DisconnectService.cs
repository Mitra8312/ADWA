
using ADWA.DataBase;
using ADWA.Models;

namespace ADWA.Services
{
	public class DisconnectService : BackgroundService
	{
		/// <summary>
		/// Метод фонового процесса, который запускает отключение удаленного доступа для пользователей каждые 24 часа
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_ = await DisconnetUsers();

				await Task.Delay(10000 * 60 * 24, stoppingToken);
			}
		}

		/// <summary>
		/// Метод для отключения удаленного доступа
		/// Возвращаемое значение используется для дальнейшего использования в логах
		/// </summary>
		/// <returns>True - если метод успешно отработал, False - в случае ошибок в исполнении</returns>
		private async Task<bool> DisconnetUsers()
		{
			try
			{
				var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

				var ldapSettings = configuration.GetSection("LdapSettings").Get<LdapSettings>();
				var logger = new Logger<ActiveDirectoryService>(new LoggerFactory());

				ActiveDirectoryService service = new ActiveDirectoryService(configuration, logger);
				DBContext dbContext = new();

				List<ApplicationUser> user = dbContext.Users.ToList();

				foreach (ApplicationUser u in user)
				{
					if (DateTime.TryParse(u.GetDateOfDisconnect(), out DateTime dateValue))
					{
						if (dateValue < DateTime.Now)
						{
							service.DisconnectUser(u.GetSamAccountName());

							dbContext.Users.Remove(u);
                        }
					}
				}

				await dbContext.SaveChangesAsync();

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
