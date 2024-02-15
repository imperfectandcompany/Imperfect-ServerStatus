using IGDiscord.Models.Discord;

namespace IGDiscord.Models.MessageInfo
{
    public class StatusMessageInfo : MessageInfo
    {
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// Message ID of server status message if already posted
        /// </summary>
        public string? MessageId { get; set; } = "";
    }
}
