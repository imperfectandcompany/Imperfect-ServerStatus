using IGDiscord.Helpers;
using IGDiscord.Models.Discord;
using IGDiscord.Models.Messages;
using IGDiscord.src.Models.Discord;
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
            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(messageInfo.MessageInterval));

            WebhookMessage discordWebhookMessage = CreateServerStatusWebhookMessage(messageInfo);

            /// Initial post
            await PostEmbedToWebhook(discordWebhookMessage, messageInfo.WebhookUri);

            while (await periodicTimer.WaitForNextTickAsync())
            {
                await PostEmbedToWebhook(discordWebhookMessage, messageInfo.WebhookUri);
            }
        }

        public async Task PostEmbedToWebhook(WebhookMessage discordWebhookMessage, string webhookUri)
        {
            try
            {
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
                };

                var body = JsonSerializer.Serialize(discordWebhookMessage, serializeOptions);

                Util.PrintLog(body);

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = (await _httpClient.PostAsync($"{webhookUri}", content)).EnsureSuccessStatusCode();

                Util.PrintLog("Sent server status message");
            }
            catch (Exception ex)
            {
                Util.PrintLog($"Failed to send: {ex.Message}");
            }
        }

        private WebhookMessage CreateServerStatusWebhookMessage(ServerStatusMessageInfo messageInfo)
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
                        Timestamp = DateOnly.FromDateTime(DateTime.Now)
                    }
                }
            };
        }
    }
}
