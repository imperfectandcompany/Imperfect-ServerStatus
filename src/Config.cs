using CounterStrikeSharp.API.Core;
using IGDiscord.Models;

namespace IGDiscord
{
    public class Config : BasePluginConfig
    {
        /// <summary>
        /// List of Webhook URL's and prefixes for Discord messages
        /// </summary>
        public List<Webhook>? Webhooks { get; set; }

        /// <summary>
        /// Interval in seconds that this Webhook is called
        /// </summary>
        public int LogInterval { get; set; }
    }
}