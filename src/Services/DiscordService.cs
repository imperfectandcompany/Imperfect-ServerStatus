using CounterStrikeSharp.API.Core;
using IGDiscord.Helpers;
using IGDiscord.Models;
using IGDiscord.Models.Discord;
using IGDiscord.Models.MessageInfo;
using IGDiscord.Services.Interfaces;
using IGDiscord.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IGDiscord.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigService _configService;
        private Config? _config;

        public DiscordService(IConfigService configService)
        {
            _httpClient = new HttpClient();
            _configService = configService;
        }

        public async Task<string> SendInitialStatusMessage(StatusMessageInfo messageInfo, WebhookMessage webhookMessage)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

            var serializedMessage = JsonSerializer.Serialize(webhookMessage, serializeOptions);

            Util.PrintLog("Sending initial status message");

            /// No message exists, send first message
            var response = await PostJsonToWebhook(serializedMessage, messageInfo);

            if (response != null)
            {
                return await GetDiscordMessageId(response);
            }
            else
            {
                Util.PrintError("Something went wrong getting the response after posting the Discord message.");
                return string.Empty;
            }
        }

        public async Task UpdateStatusMessage(StatusMessageInfo messageInfo, WebhookMessage webhookMessage)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

            /// Add message ID to Webhook URI
            var messageEditUri = messageInfo.WebhookUri + "/messages/" + messageInfo.MessageId;

            var serializedMessage = JsonSerializer.Serialize(webhookMessage, serializeOptions);

            await PatchJsonToWebhook(serializedMessage, messageEditUri);
        }

        public async Task<HttpResponseMessage> PostJsonToWebhook(string serializedMessage, StatusMessageInfo messageInfo)
        {
            try
            {
                var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return (await _httpClient.PostAsync($"{messageInfo.WebhookUri}?wait=true", content)).EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Util.PrintError($"Failed to send: {ex.Message}");
            }

            return null;
        }

        private async Task<string> GetDiscordMessageId(HttpResponseMessage response)
        {
            var deserializedResponse = JsonSerializer.Deserialize<WebhookResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (deserializedResponse != null
                && !string.IsNullOrEmpty(deserializedResponse.Id))
            {
                return deserializedResponse.Id;
            }
            else
            {
                return string.Empty;
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
                Util.PrintError($"Failed to send: {ex.Message}");
            }
        }

        public WebhookMessage CreateWebhookMessage(StatusMessageInfo messageInfo, StatusData statusData)
        {
            messageInfo.MessageEmbed.Timestamp = statusData.Timestamp;

            return new WebhookMessage
            {
                Content = "",
                Embeds = new List<Embed>
                {
                    messageInfo.MessageEmbed
                }
            };
        }

        public WebhookMessage UpdateWebhookMessage(WebhookMessage webhookMessage, StatusData statusData)
        {
            if (webhookMessage.Embeds != null)
            {
                var embed = webhookMessage.Embeds.FirstOrDefault();

                if (embed != null)
                {
                    embed.Description = "Updated description on " + statusData.Timestamp;
                    embed.Timestamp = statusData.Timestamp;
                }
            }

            return webhookMessage;
        }
    }
}
