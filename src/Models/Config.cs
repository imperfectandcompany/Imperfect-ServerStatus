using CounterStrikeSharp.API.Core;
using ImperfectServerStatus.Models.Discord;
using ImperfectServerStatus.Models.MessageInfo;
using System.Text.Json.Serialization;

namespace ImperfectServerStatus.Models
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