namespace IGDiscord.Models
{
    public class Webhook
    {
        public string Type { get; set; }

        public string? MessagePrefix { get; set; }

        public string? WebHookUri { get; set; }
    }
}