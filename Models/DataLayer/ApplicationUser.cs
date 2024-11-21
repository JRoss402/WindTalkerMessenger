using Microsoft.AspNetCore.Identity;

namespace WindTalkerMessenger.Models.DataLayer
{
    public class ApplicationUser : IdentityUser
    {
        public string ChatName { get; set; }
    }
}
