namespace ImperfectServerStatus.Models
{
    public class StatusData
    {
        public string ServerName { get; set; } = "";

        public string IpAddress { get; set; } = "";

        public bool ServerOnline { get; set; } = true;

        public string MapName { get; set; } = "";

        public DateTime Timestamp { get; set; }
    }
}
