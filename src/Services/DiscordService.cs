using CounterStrikeSharp.API.Core;
using IGDiscord.Helpers;
using IGDiscord.Models;
using IGDiscord.Models.Discord;
using IGDiscord.Models.MessageInfo;
using IGDiscord.Services.Interfaces;
using IGDiscord.Utils;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace IGDiscord.Services
{
    public class DiscordService : IDiscordService
    {
        public async Task<string> CreateStatusMessageAsync(StatusMessageInfo messageInfo, WebhookMessage webhookMessage)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

            var serializedMessage = JsonSerializer.Serialize(webhookMessage, serializeOptions);

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

        public async Task UpdateStatusMessageAsync(StatusMessageInfo messageInfo, WebhookMessage webhookMessage)
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

        public WebhookMessage CreateWebhookMessage(StatusMessageInfo statusMessageInfo, StatusData statusData)
        {
            return new WebhookMessage
            {
                Content = null,
                //ActionRowComponents = new List<ActionRowComponent>()
                //{
                //    new ActionRowComponent()
                //    {
                //        Type = 1,
                //        Components = new List<ButtonComponent>()
                //        {
                //            //CreateButtonComponent(statusMessageInfo)
                //        }
                //    }
                //},
                Embeds = new List<Embed>
                {
                    CreateEmbed(statusMessageInfo, statusData)
                }
            };
        }

        public WebhookMessage UpdateWebhookMessage(WebhookMessage webhookMessage, StatusData statusData)
        {
            if (webhookMessage.Embeds != null)
            {
                var statusEmbed = webhookMessage.Embeds.FirstOrDefault();

                statusEmbed = UpdateEmbed(statusEmbed, statusData);
            }

            return webhookMessage;
        }

        private async Task<HttpResponseMessage> PostJsonToWebhook(string serializedMessage, StatusMessageInfo messageInfo)
        {
            try
            {
                var webhookRequestUri = $"{messageInfo.WebhookUri}?wait=true";

                var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");

                using HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return (await httpClient.PostAsync(webhookRequestUri, content)).EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Util.PrintError($"Failed to send: {ex.Message}");
            }

            return null;
        }

        private async Task PatchJsonToWebhook(string serializedMessage, string webhookUri)
        {
            try
            {
                var content = new StringContent(serializedMessage, Encoding.UTF8, "application/json");

                using HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));

                HttpResponseMessage response = (await httpClient.PatchAsync($"{webhookUri}", content)).EnsureSuccessStatusCode();
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

        private Embed CreateEmbed(StatusMessageInfo statusMessageInfo, StatusData statusData)
        {
            var connectUrl = "";
            if (statusData.IpAddress != null)
            {
                connectUrl = "https://cs2browser.com/connect/" + statusData.IpAddress;
            }

            var embed = new Embed()
            {
                Title = statusData.ServerName ?? "Server Name",
                Description = "",
                Type = "rich",
                Url = connectUrl,
                Color = 16724530,
                Timestamp = DateTime.Now,
                Fields = new List<EmbedField>(){
                    new EmbedField(){
                        Name = "Status",
                        Value = "Online \uD83D\uDFE2",
                        Inline = true
                    },
                    new EmbedField()
                    {
                        Name = "Map",
                        Value = statusData.MapName ?? "",
                        Inline = true
                    },
                    new EmbedField()
                    {
                        Name = "IP Address",
                        Value = statusData.IpAddress ?? "",
                        Inline = true
                    },
                    new EmbedField()
                    {
                        Name = "Connect Link",
                        Value = $"[Click me]({connectUrl})",

                        Inline = true
                    }
                }
            };

            return embed;
        }

        private Embed? UpdateEmbed(Embed? statusEmbed, StatusData statusData)
        {
            if (statusEmbed != null)
            {
                statusEmbed.Title = statusData.ServerName;
                statusEmbed.Timestamp = statusData.Timestamp;
                
                
                var connectUrl = "";
                if (statusData.IpAddress != null)
                {
                    connectUrl = "https://cs2browser.com/connect/" + statusData.IpAddress;
                }
                statusEmbed.Url = connectUrl;

                var mapNameField = statusEmbed.Fields.FirstOrDefault(f => f.Name == "Map");

                if (mapNameField != null)
                {
                    mapNameField.Value = statusData.MapName ?? "";
                }

                var ipAddressField = statusEmbed.Fields.FirstOrDefault(f => f.Name == "IP Address");

                if (ipAddressField != null)
                {
                    ipAddressField.Value = statusData.IpAddress ?? "";
                }

                var serverOnlineStatusField = statusEmbed.Fields.FirstOrDefault(f => f.Name == "Status");

                if (serverOnlineStatusField != null)
                {
                    if (statusData.ServerOnline is false)
                    {
                        serverOnlineStatusField.Value = "Offline \uD83D\uDD34";
                    }
                }

                var connectLinkField = statusEmbed.Fields.FirstOrDefault(f => f.Name == "Connect Link");

                if (connectLinkField != null)
                {
                    connectLinkField.Value = $"[Click me]({connectUrl})";
                }
            }

            return statusEmbed;
        }

        private ButtonComponent CreateButtonComponent(StatusMessageInfo statusMessageInfo)
        {
            var buttonComponent = new ButtonComponent()
            {
                Type = 2,
                Style = 5,
                Label = "Connect to server",
                Url = "",
                Disabled = false,
                Emoji = new Emoji()
                {
                    Id = null,
                    Name = "🔗"
                }
            };

            return buttonComponent;
        }
    }
}
