using System.Text.Json.Serialization;

namespace ImperfectServerStatus.Models.Discord
{
    public class ActionRowComponent
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("components")]
        public List<ButtonComponent> Components { get; set; }
    }
}
