using System.ComponentModel.DataAnnotations;

namespace WindTalkerMessenger.Models.ViewModels
{
    public class ChatOptionsViewModel
    {
        public int GuestId { get; set; }

        [Required]
        public string GuestName { get; set; }
    }
}
