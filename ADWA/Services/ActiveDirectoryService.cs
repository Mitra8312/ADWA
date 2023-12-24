// Services\ActiveDirectoryService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.AccountManagement;
using ADWA.Models;
using System.DirectoryServices;
using ADWA.Controllers;
using System.Diagnostics;

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
            _context = new PrincipalContext(ContextType.Domain, _ldapSettings.Server, _ldapSettings.SearchBase, _ldapSettings.BindDN, _ldapSettings.BindPassword);
        }
        catch (Exception ex)
        {
            throw new Exception($"Контроллер домена недоступен! - {ex.Message}"); ;  
        } 
    }
    public List<UserPrincipal> GetUsersWithDialInEnabled()
    {
        List<UserPrincipal> usersWithDialIn = [];

        using (PrincipalSearcher principalSearcher = new(new UserPrincipal(_context)))
        {
            foreach (var user in principalSearcher.FindAll())
            {
#pragma warning disable CA1416 // Проверка совместимости платформы
                DirectoryEntry? de = (DirectoryEntry)user.GetUnderlyingObject();
#pragma warning restore CA1416 // Проверка совместимости платформы
                if (de.Properties.Contains("msNPAllowDialin") &&
                    de.Properties["msNPAllowDialin"].Value != null &&
                    (bool)de.Properties["msNPAllowDialin"].Value)
                {
                    usersWithDialIn.Add((UserPrincipal)user);
                }
            }
        }
        return usersWithDialIn;
    }
    public List<ApplicationUser> GetUsers()
    {
        List<ApplicationUser> users = new List<ApplicationUser>();
        using (var searcher = new PrincipalSearcher(new UserPrincipal(_context)))
        {
            foreach (var user in searcher.FindAll())
            {
                var adUser = user as UserPrincipal;
                if (adUser != null)
                {
                    var directoryEntry = (DirectoryEntry)adUser.GetUnderlyingObject();
                    var msNPAllowDialin = directoryEntry.Properties["msNPAllowDialin"].Value;
                    users.Add(new ApplicationUser
                    {
                        GivenName = adUser.Name,
                        Surname = adUser.DisplayName,
                        SamAccountName = adUser.SamAccountName,
                        IsDialInEnabled = msNPAllowDialin != null && (bool)msNPAllowDialin
                    });
                }
            }
            return users;
        }
    }
   public void UpdateDialInStatus (string SamAccountName, bool isEnabled)
    {
        Console.WriteLine("TUT");
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
    public UserPrincipal GetUserByLogin(string login)
    {
        try
        {
            return UserPrincipal.FindByIdentity(_context, IdentityType.SamAccountName, login);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Пользователь не найден");
            throw;
        }
    }
    public bool Authenticate(string username, string password)
    {
        try
        {
            // Use your logic to authenticate against Active Directory
            // For example, you can use PrincipalContext.ValidateCredentials method

            return _context.ValidateCredentials(username, password);
        }
        catch (Exception ex)
        {
            // Handle authentication failure or any exceptions
            _logger.LogError($"Authentication failed: {ex.Message}");
            return false;
        }
    }
}

