using IGDiscord.Models.Messages;

namespace IGDiscord.Models.Messages
{
    public class ServerStatusMessageInfo : MessageInfo
    {
        /// <summary>
        /// Interval in seconds that this Webhook is called. 0 if not repeated.
        /// </summary>
        public int LogInterval { get; set; } = 0;
    }
}
