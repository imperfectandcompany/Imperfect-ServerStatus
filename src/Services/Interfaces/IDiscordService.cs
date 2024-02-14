using IGDiscord.Models;
using IGDiscord.Models.Discord;
using IGDiscord.Models.MessageInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.Services.Interfaces
{
    public interface IDiscordService
    {
        Task<string> CreateStatusMessageAsync(StatusMessageInfo messageInfo, WebhookMessage webhookMessage);

        Task UpdateStatusMessageAsync(StatusMessageInfo messageInfo, WebhookMessage webhookMessage);

        WebhookMessage CreateWebhookMessage(StatusMessageInfo statusMessageInfo, StatusData statusData);

        WebhookMessage UpdateWebhookMessage(WebhookMessage webhookMessage, StatusData statusData);
    }
}
