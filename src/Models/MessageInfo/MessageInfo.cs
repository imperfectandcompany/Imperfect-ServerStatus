namespace IGDiscord.Models.MessageInfo
{
    public class MessageInfo
    {
        public required string Type { get; set; }

        /// <summary>
        /// Prefix of message posted to Discord
        /// </summary>
        public string Prefix { get; set; } = "IG";

        /// <summary>
        /// Webhook Uri
        /// </summary>
        public required string WebhookUri { get; set; }
    }
}
