using ImperfectServerStatus.Models.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ImperfectServerStatus.Models.Discord
{
    public class WebhookMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("components")]
        public List<ActionRowComponent> ActionRowComponents{ get; set; }

        [JsonPropertyName("embeds")]
        public List<Embed> Embeds { get; set; }

    }
}
