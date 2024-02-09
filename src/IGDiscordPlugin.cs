using CounterStrikeSharp.API.Core;
using IGDiscord.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IGDiscord;

public class IGDiscordPlugin : BasePlugin
{
    public override string ModuleName => "Imperfect Gamers Discord Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "Plugin for Discord Webhooks for Imperfect Gamers";

    private static Config? _config;
    private static readonly HttpClient _httpClient;

    static IGDiscordPlugin()
    {
        _httpClient = new HttpClient();
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    public override void Load(bool hotReload)
    {
        GetOrCreateConfig();

        if (_config != null)
        {

            if (_config.Webhooks != null)
            {
                foreach (var webhook in _config.Webhooks)
                {
                    Task.Run(async () =>
                    {
                        await SendDiscordMessage(_config.LogInterval, webhook);
                    });
                }
            }
        }
        else
        {
            Console.WriteLine("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }

    private async Task SendDiscordMessage(int interval, Webhook webhook)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(interval));

        while (await periodicTimer.WaitForNextTickAsync())
        {
            Console.WriteLine("Sending server status");

            var message = webhook.MessagePrefix + "Server status";

            try
            {
                var body = JsonSerializer.Serialize(new { content = message });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = (await _httpClient.PostAsync($"{webhook.WebHookUrl}", content)).EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send: {ex.Message}");
            }

        }
    }

    private void GetOrCreateConfig()
    {
        var configPath = Path.Join(ModuleDirectory, "Config.json");

        if (!File.Exists(configPath))
        {
            // This is the first time running this plugin, create the config file
            var configData = new Config()
            {
                Webhooks = new List<Webhook>()
                {
                    new Webhook()
                    {
                        MessagePrefix = "### SAMPLE MESSAGE PREFIX ###",
                        WebHookUrl = "https://discord.com/api/webhooks/###############/#################",
                    }
                },
                LogInterval = 300
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var serializedConfigData = JsonSerializer.Serialize(configData, jsonOptions);

            File.WriteAllText(configPath, serializedConfigData);

            _config = configData;
        }
        else
        {
            var configData = File.ReadAllText(configPath);

            _config = JsonSerializer.Deserialize<Config>(configData);
        }
    }
}