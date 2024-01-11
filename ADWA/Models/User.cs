namespace ADWA.Models
{
    public class User
    {
        public int Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string SamAccountName { get; set; }
        public bool IsDialInEnabled { get; set; }
        public DateTime AccessExpirationDate { get; set; }
    }

}
