using System.Text.Json.Serialization;

namespace ADWA.Models
{
	public class OrganisationUnit
    {
		public OrganisationUnit(string oUPath, string name, string parent)
		{
			Name = name;
			OUPath = oUPath;
			Children = new List<OrganisationUnit>();
			Users = new List<ApplicationUser>();
			Parent = parent;
		}

		[JsonPropertyName("OUPath")]
		private string OUPath { get; set; }

		[JsonPropertyName("Name")]
		private string Name { get; set; }

		[JsonPropertyName("Children")]
		private List<OrganisationUnit> Children { get; set; }

		[JsonPropertyName("Users")]
		private List<ApplicationUser> Users { get; set; }

		private string Parent;

		public string GetParent()
		{
			return Parent;
		}

		public void SetParent(string value)
		{
			Parent = value;
		}

		public string GetOUPath()
        {
            return OUPath;
        }

        public void SetOUPath(string NamePath)
        {
            OUPath = NamePath;
        }
        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public List<OrganisationUnit> GetChildren()
        {
            return Children;
        }

        public void AddChild(OrganisationUnit unit)
        {
            Children.Add(unit);
        }

        public List<ApplicationUser> GetUsers()
        {
            return Users;
        }

        public void AddUser(ApplicationUser user)
        {
            Users.Add(user);
        }
    }
}
