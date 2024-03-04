using System.Text.Json.Serialization;

namespace ImperfectServerStatus.Models.Discord
{
    public class WebhookMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("components")]
        public List<ActionRowComponent> ActionRowComponents { get; set; }

        [JsonPropertyName("embeds")]
        public List<Embed> Embeds { get; set; }

    }
}
