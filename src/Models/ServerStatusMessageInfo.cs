using IGDiscord.Models;
using IGDiscord.src.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.Models
{
    public class ServerStatusMessageInfo : MessageInfo
    {
        /// <summary>
        /// Interval in seconds that this Webhook is called. 0 if not repeated.
        /// </summary>
        public int LogInterval { get; set; } = 0;
    }
}
