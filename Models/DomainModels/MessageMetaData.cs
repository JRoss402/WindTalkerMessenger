using System.Diagnostics;

namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageMetaData
    {
        public int MessageMetaDataId { get; set; }
        public string? MessageFamilyUID { get; set; }
        public string? SenderGUID { get; set; }
        public string? ReceiverGUID { get; set; }

        //public string? MessageFamilyUID { get; set; }

    }
}
