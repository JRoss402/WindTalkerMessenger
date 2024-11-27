using NuGet.Protocol.Plugins;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Models.DomainModels
{
    public class Sender : IUserInformation
    {
        public string? ConnectionId { get; set; }
        public string? ChatName { get; set; }
        public string? EMail { get; set; }

    }
}
