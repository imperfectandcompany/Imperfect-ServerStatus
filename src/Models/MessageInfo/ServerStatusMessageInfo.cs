namespace IGDiscord.Models.MessageInfo
{
    public class ServerStatusMessageInfo : MessageInfo
    {
        /// <summary>
        /// Interval in seconds that this Webhook is called. 0 if not repeated.
        /// </summary>
        public int MessageInterval { get; set; } = 0;
    }
}
