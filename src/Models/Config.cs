using CounterStrikeSharp.API.Core;
using IGDiscord.Models.Discord;
using IGDiscord.Models.MessageInfo;
using System.Text.Json.Serialization;

namespace IGDiscord.Models
{
    public class Config : BasePluginConfig
    {
        /// <summary>
        /// Config version number
        /// </summary>
        [JsonPropertyName("ConfigVersion")]
        public override int Version { get; set; } = 1;

        /// <summary>
        /// Server status message information
        /// </summary>
        public StatusMessageInfo StatusInfo { get; set; } = new();
    }
}