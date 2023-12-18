// Services\ActiveDirectoryService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using ADWA.Models;
using System.DirectoryServices;

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
            _logger.LogError($"Ошибка подключения к AD: {ex.Message}");
            throw; // rethrow the exception
        }
    }

    public List<UserPrincipal> GetUsersWithDialInEnabled()
    {
        List<UserPrincipal> usersWithDialIn = new List<UserPrincipal>();

        using (PrincipalSearcher principalSearcher = new PrincipalSearcher(new UserPrincipal(_context)))
        {
            foreach (var user in principalSearcher.FindAll())
            {
                DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;
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
    public List<UserPrincipal> GetUsers()
    {
        List<UserPrincipal> users = new List<UserPrincipal>();
        using (PrincipalSearcher principalSearcher = new PrincipalSearcher(new UserPrincipal(_context)))
        {
            foreach (var user in principalSearcher.FindAll())
            {
                DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;
                
                    users.Add((UserPrincipal)user);
                
            }
        }
        return users;

    }



}
