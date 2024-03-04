namespace ImperfectServerStatus.Models.MessageInfo
{
    public class StatusMessageInfo : MessageInfo
    {
        /// <summary>
        /// Message ID of server status message if already posted
        /// </summary>
        public string? MessageId { get; set; } = "";
    }
}
