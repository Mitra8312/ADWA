using ADWA.Models;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

public class ActiveDirectoryService
{
	private readonly LdapSettings _ldapSettings;
	private readonly PrincipalContext _context;
	private readonly ILogger<ActiveDirectoryService> _logger;

	public ActiveDirectoryService(IConfiguration configuration, ILogger<ActiveDirectoryService> logger)
	{
		_ldapSettings = configuration.GetSection("LdapSettings").Get<LdapSettings>();
		_logger = logger;

		try
		{
			// Use settings from appsettings.json to create the context
			_context = new PrincipalContext(ContextType.Domain, _ldapSettings.Server, _ldapSettings.SearchBase, _ldapSettings.UserName, _ldapSettings.BindPassword);
		}
		catch (Exception ex)
		{
			throw new Exception($"Контроллер домена недоступен! - {ex.Message}");
		}
	}

	public List<UserPrincipal>? GetUsersWithDialInEnabled()
	{
		try
		{
			List<UserPrincipal> usersWithDialIn = [];

			using (PrincipalSearcher principalSearcher = new(new UserPrincipal(_context)))
			{
				foreach (Principal? user in principalSearcher.FindAll())
				{
					DirectoryEntry? directoryEntry = (DirectoryEntry)user.GetUnderlyingObject();
					if (directoryEntry.Properties.Contains("msNPAllowDialin") && directoryEntry.Properties["msNPAllowDialin"].Value != null && (bool)directoryEntry.Properties["msNPAllowDialin"].Value)
					{
						usersWithDialIn.Add((UserPrincipal)user);
					}
				}
			}
			return usersWithDialIn;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	public void UpdateDialInStatus(string SamAccountName, bool isEnabled)
	{
		try
		{
			// Ищем пользователя по его логину
			UserPrincipal user = UserPrincipal.FindByIdentity(_context, IdentityType.SamAccountName, SamAccountName);
			Console.WriteLine(user);

			if (user != null)
			{
				// Получаем объект DirectoryEntry для пользователя
				DirectoryEntry directoryEntry = (DirectoryEntry)user.GetUnderlyingObject();

				// Обновляем значение свойства msNPAllowDialin
				directoryEntry.Properties["msNPAllowDialin"].Value = isEnabled;

				// Применяем изменения
				Console.WriteLine(isEnabled);
				directoryEntry.CommitChanges();
			}
			else
			{
				// Обработка случая, когда пользователь не найден
				Console.WriteLine($"User {SamAccountName} not found in Active Directory");
				// Возможно, стоит бросить исключение, если требуется детальное уведомление об ошибке
			}
		}
		catch (Exception ex)
		{
			// Обработка ошибок при обновлении состояния Dial-in
			Console.WriteLine($"Error updating Dial-in status for user {SamAccountName}: {ex.Message}");
			// Возможно, стоит бросить исключение или добавить логирование для детального уведомления об ошибке
		}
	}

	public void DisconnectUser(string SamAccountName)
	{
		try
		{
			// Ищем пользователя по его логину
			UserPrincipal user = UserPrincipal.FindByIdentity(_context, IdentityType.SamAccountName, SamAccountName);
			Console.WriteLine(user);

			if (user != null)
			{
				DirectoryEntry directoryEntry = (DirectoryEntry)user.GetUnderlyingObject();

				directoryEntry.Properties["msNPAllowDialin"].Value = false;

				directoryEntry.CommitChanges();
			}
			else
			{
				Console.WriteLine($"User {SamAccountName} not found in Active Directory");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error updating Dial-in status for user {SamAccountName}: {ex.Message}");
		}
	}

	public UserPrincipal GetUserByLogin(string login)
	{
		try
		{
			return UserPrincipal.FindByIdentity(_context, IdentityType.SamAccountName, login);
		}
		catch (Exception)
		{
			Console.WriteLine($"Пользователь не найден");
			throw;
		}
	}

	public bool Authenticate(string username, string password)
	{
		UserPrincipal user = UserPrincipal.FindByIdentity(_context, IdentityType.SamAccountName, username);
		if (user != null)
		{
			// Поиск группы по имени
			GroupPrincipal group = GroupPrincipal.FindByIdentity(_context, "adwa-users");

			if (group != null)
			{
				// Проверка членства пользователя в группе
				return _context.ValidateCredentials(username, password) && user.IsMemberOf(group);
			}
			else
			{
				// Группа не найдена
				return false;
			}
		}
		else
		{
			// Пользователь не найден
			return false;
		}
	}

	/// <summary>
	/// Получение всех пользователей из AD
	/// </summary>
	/// <returns>Список всех пользователей</returns>
	public List<ApplicationUser> GetUsers()
	{
		List<ApplicationUser> users = [];

		using PrincipalSearcher searcher = new(new UserPrincipal(_context));
		foreach (Principal? user in searcher.FindAll())
		{
			if (user is UserPrincipal adUser)
			{
				DirectoryEntry directoryEntry = (DirectoryEntry)adUser.GetUnderlyingObject();
				object? msNPAllowDialin = directoryEntry.Properties["msNPAllowDialin"].Value;

				users.Add(new ApplicationUser(adUser.Name, adUser.DisplayName, adUser.SamAccountName, msNPAllowDialin != null && (bool)msNPAllowDialin, adUser.DistinguishedName));
			}
		}

		return users;
	}
	/// <summary>
	/// Получение пользователей без удаленного доступа
	/// </summary>
	/// <returns>Список пользователей без удаленного доступа</returns>
	public List<ApplicationUser> GetUsersWithoutRemoteAccess()
	{
		List<ApplicationUser> users = [];

		foreach (ApplicationUser user in GetUsers())
		{
			if (user.IsDialInEnabled == null || user.IsDialInEnabled == false)
			{
				users.Add(user);
			}
		}
		return users;
	}
}

