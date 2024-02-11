using IGDiscord.Helpers;
using IGDiscord.Models;
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
        private readonly HttpClient _httpClient;
        private readonly ConfigService _configService;
        private Config? _config;

        public DiscordService(ConfigService configService)
        {
            _httpClient = new HttpClient();
            _configService = configService;
        }

        public async Task SendServerStatusMessage(Config config)
        {
            if (config == null)
            {
                Util.PrintError($"Something went wrong with parsing the config.");
            }
            else
            {
                _config = config;
            }

            var messageInfo = _config.ServerStatusMessage;

            if (messageInfo != null)
            {
                WebhookMessage discordWebhookMessage = CreateMessage(messageInfo);

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
                };

                if (string.IsNullOrEmpty(messageInfo.MessageId))
                {
                    var serializedMessage = JsonSerializer.Serialize(discordWebhookMessage, serializeOptions);

                    /// No message exists, send first message
                    await PostJsonToWebhook(serializedMessage, messageInfo);
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

        public async Task PostJsonToWebhook(string serializedMessage, ServerStatusMessageInfo messageInfo)
        {
            try
            {
                var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = (await _httpClient.PostAsync($"{messageInfo.WebhookUri}?wait=true", content)).EnsureSuccessStatusCode();

                messageInfo.MessageId = await GetDiscordMessageId(response);

                _config.ServerStatusMessage = messageInfo;
                _configService.UpdateConfig(_config);
            }
            catch (Exception ex)
            {
                Util.PrintError($"Failed to send: {ex.Message}");
            }
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
