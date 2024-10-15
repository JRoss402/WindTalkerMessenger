using Microsoft.AspNetCore.Identity;

namespace WindTalkerMessenger.Models.DataLayer
{
    public class ApplicationUser : IdentityUser
    {
        public string UserUserName { get; set; }
        public string? UserIpAddress { get; set; }
        //public string? UserEmail { get; set; }

    }
}
