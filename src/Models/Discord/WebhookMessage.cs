using IGDiscord.Models.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.Models.Discord
{
    public class WebhookMessage
    {
        public string? Content { get; set; }

        public List<Embed> Embeds { get; set; }

    }
}
