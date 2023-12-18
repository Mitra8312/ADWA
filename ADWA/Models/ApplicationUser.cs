using System.DirectoryServices.AccountManagement;

namespace ADWA.Models
{
    public class ApplicationUser
    {

        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string SamAccountName { get; set; }
        public bool IsDialInEnabled { get; set; }

  
    }
}
