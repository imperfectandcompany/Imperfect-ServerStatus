using IGDiscord.Helpers;
using IGDiscord.Models.Discord;
using IGDiscord.Models.MessageInfo;
using IGDiscord.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IGDiscord.Services
{
    public class DiscordService
    {
        private static readonly HttpClient _httpClient;

        static DiscordService()
        {
            _httpClient = new HttpClient();
        }
        public async Task SendServerStatusMessage(ServerStatusMessageInfo messageInfo)
        {
            WebhookMessage discordWebhookMessage = CreateMessage(messageInfo);

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

            var jsonSerializedMessage = JsonSerializer.Serialize(discordWebhookMessage, serializeOptions);

            if (string.IsNullOrEmpty(messageInfo.MessageId))
            {
                var serializedMessage = JsonSerializer.Serialize(discordWebhookMessage, serializeOptions);

                /// No message exists, send first message
                await PostJsonToWebhook(serializedMessage, messageInfo.WebhookUri);
            }
            else
            {
                /// Message already exists, update it periodically

                /// Add message ID to Webhook URI
                messageInfo.WebhookUri = messageInfo.WebhookUri + "/messages/" + messageInfo.MessageId;

                var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(messageInfo.MessageInterval));
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    discordWebhookMessage = UpdateMessage(discordWebhookMessage);

                    var serializedMessage = JsonSerializer.Serialize(discordWebhookMessage, serializeOptions);

                    await PatchJsonToWebhook(serializedMessage, messageInfo.WebhookUri);
                }
            }
        }

        public async Task PostJsonToWebhook(string serializedMessage, string webhookUri)
        {
            try
            {
                var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = (await _httpClient.PostAsync($"{webhookUri}", content)).EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Util.PrintLog($"Failed to send: {ex.Message}");
            }
        }

        public async Task PatchJsonToWebhook(string serializedMessage, string webhookUri)
        {
            try
            {
                var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));

                HttpResponseMessage response = (await _httpClient.PatchAsync($"{webhookUri}", content)).EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Util.PrintLog($"Failed to send: {ex.Message}");
            }
        }

        public WebhookMessage CreateMessage(ServerStatusMessageInfo messageInfo)
        {
            return new WebhookMessage
            {
                Content = "",
                Embeds = new List<Embed>
                {
                    new Embed
                    {
                        Title = "Server Status Title",
                        Description = "This is where the description of the server status will go.",
                        Timestamp = DateTime.Now
                    }
                }
            };
        }

        public WebhookMessage UpdateMessage(WebhookMessage webhookMessage)
        {
            if (webhookMessage.Embeds != null)
            {
                var embed = webhookMessage.Embeds.FirstOrDefault();

                if (embed != null)
                {
                    embed.Description = "Updated description on " + DateTime.Now;
                    embed.Timestamp = DateTime.Now;
                }
            }

            return webhookMessage;
        }
    }
}
