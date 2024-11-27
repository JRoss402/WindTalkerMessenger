namespace WindTalkerMessenger.Models.DomainModels
{
	public class Client : IUserInformation
	{
		public string? ConnectionId { get; set; }
		public string? ChatName { get; set; }
	}
}
