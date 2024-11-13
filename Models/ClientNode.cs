namespace WindTalkerMessenger.Models
{
    public class ClientNode
    {
        public string NodeName { get; set; }
        public int LeftNodeReceive { get; set; }
        public DateTime? oldTime { get; set; }
        public DateTime? newTime { get; set; }
        public int RightNodeSend { get; set; }


        public TimeSpan TimeSpread()
        {
            var span = newTime - oldTime;

            return (TimeSpan)span;
        }
    }



}
