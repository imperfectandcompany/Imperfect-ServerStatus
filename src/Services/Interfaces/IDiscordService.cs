using IGDiscord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.Services.Interfaces
{
    public interface IDiscordService
    {
        Task SendStatusMessage(Config config);
    }
}
