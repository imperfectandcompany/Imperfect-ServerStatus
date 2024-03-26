using CounterStrikeSharp.API.Core;
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
        public override int Version { get; set; } = 2;

        /// <summary>
        /// The server IP to display and create connect links to
        /// </summary>
        public string ServerIp { get; set; } = "";

        /// <summary>
        /// Server status message information
        /// </summary>
        public StatusMessageInfo StatusInfo { get; set; } = new();
    }
}