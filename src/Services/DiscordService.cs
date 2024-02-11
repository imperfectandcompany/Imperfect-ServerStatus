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

        public async Task SendStatusMessage(Config config, string configPath)
        {
            _config = config;

            var messageInfo = _config.StatusMessageInfo;

            if (messageInfo != null)
            {
                WebhookMessage discordWebhookMessage = CreateMessage(messageInfo);

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
                };

                if (string.IsNullOrEmpty(messageInfo.MessageId))
                {
                    var serializedMessage = JsonSerializer.Serialize(discordWebhookMessage, serializeOptions);

                    /// No message exists, send first message
                    var response = await PostJsonToWebhook(serializedMessage, messageInfo);

                    if (response != null)
                    {
                        messageInfo.MessageId = await GetDiscordMessageId(response);

                        _config.StatusMessageInfo = messageInfo;

                        _configService.UpdateConfig(_config, configPath);
                    }
                    else
                    {
                        Util.PrintError("Something went wrong getting the response after posting the Discord message.");
                    }
                }

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

        public WebhookMessage CreateMessage(StatusMessageInfo messageInfo)
        {
            messageInfo.MessageEmbed.Timestamp = DateTime.Now;

            return new WebhookMessage
            {
                Content = "",
                Embeds = new List<Embed>
                {
                    messageInfo.MessageEmbed
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
