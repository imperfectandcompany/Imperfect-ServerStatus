using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IGDiscord.Models.Discord
{
    public class ActionRowComponent
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("components")]
        public List<ButtonComponent> Components { get; set; }
    }
}
