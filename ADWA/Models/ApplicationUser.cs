using System.Text.Json.Serialization;

namespace ADWA.Models
{
	public class ApplicationUser
	{
		public string GivenName { get; set; }

		public string? Surname { get; set; }

		public string SamAccountName { get; set; }

		private string Password { get; set; }

		public bool IsDialInEnabled { get; set; }

		public string DistinguishedName { get; set; }

		public string? DateOfDisconnect { get; set; }

		public ApplicationUser() { }

		public ApplicationUser(string givenName, string surname, string samAccountName, bool isDialInEnabled, string distinguishedName)
		{
			GivenName = givenName;
			Surname = surname;
			SamAccountName = samAccountName;
			IsDialInEnabled = isDialInEnabled;
			DistinguishedName = distinguishedName;
			Password = "";
		}

		public string GetDateOfDisconnect()
		{
			return DateOfDisconnect;
		}

		public void SetDateOfDisconnect(string value)
		{
			DateOfDisconnect = value;
		}

		public bool GetIsDialInEnabled()
		{
			return IsDialInEnabled;
		}

		public void SetIsDialInEnabled(bool value)
		{
			IsDialInEnabled = value;
		}

		public string GetPassword()
		{
			return Password;
		}

		public void SetPassword(string value)
		{
			Password = value;
		}

		public string GetDistinguishedName()
		{
			return DistinguishedName;
		}

		public void SetDistinguishedName(string value)
		{
			DistinguishedName = value;
		}

		public string GetGivenName()
		{
			return GivenName;
		}

		public void SetGivenName(string value)
		{
			GivenName = value;
		}

		public string GetSurname()
		{
			return Surname;
		}

		public void SetSurname(string value)
		{
			Surname = value;
		}
		public string GetSamAccountName()
		{
			return SamAccountName;
		}

		public void SetSamAccountName(string value)
		{
			SamAccountName = value;
		}
	}
}
