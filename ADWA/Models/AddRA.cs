namespace ADWA.Models
{
	public class AddRA
	{
		public string DateOfDisconnect { get; set; }
		public string selectUser { get; set; }

        public AddRA() { } 

        public AddRA(string name, string date)
        {
            selectUser = name;
            DateOfDisconnect = date;
        }
    }
}
