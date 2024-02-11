using IGDiscord.Models.Discord;

namespace IGDiscord.Models.MessageInfo
{
    public class StatusMessageInfo : MessageInfo
    {
        /// <summary>
        /// Message ID of server status message if already posted
        /// </summary>
        public string? MessageId { get; set; } = "";

        /// <summary>
        /// Interval in seconds that this Webhook is called. 0 if not repeated.
        /// </summary>
        public int MessageInterval { get; set; } = 0;

        /// <summary>
        /// The embed of for the status message
        /// </summary>
        public Embed MessageEmbed { get; set; } = new();
    }
}
