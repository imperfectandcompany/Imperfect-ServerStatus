using IGDiscord.Models.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.src.Models.Discord
{
    public class WebhookMessage
    {
        public string? Content { get; set; }

        public required List<Embed> Embeds { get; set; }

    }
}
