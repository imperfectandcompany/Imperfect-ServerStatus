using IGDiscord.Models.Discord;
using IGDiscord.Models.Messages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IGDiscord.src.Services
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
            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(messageInfo.LogInterval));

            Embed message = CreateServerStatusMessageEmbed(messageInfo);

            while (await periodicTimer.WaitForNextTickAsync())
            {
                await SendWebhookMessage(message, messageInfo.WebhookUri);
            }
        }

        public async Task SendWebhookMessage(Embed message, string webhookUri)
        {
            try
            {
                var body = JsonSerializer.Serialize(new { content = message });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = (await _httpClient.PostAsync($"{webhookUri}", content)).EnsureSuccessStatusCode();

                Console.WriteLine("Sent server status message");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send: {ex.Message}");
            }
        }

        private Embed CreateServerStatusMessageEmbed(ServerStatusMessageInfo messageInfo)
        {
            return new Embed{
                Title = "Server Status Title",
                Description = "This is where the description of the server status will go."
            };
        }
    }
}
