using CounterStrikeSharp.API.Core;
using IGDiscord.Models.MessageInfo;
using IGDiscord.Models;
using IGDiscord.Services;
using IGDiscord.Utils;
using System.Text.Json;

namespace IGDiscord;

public class IGDiscordPlugin : BasePlugin
{
    public override string ModuleName => "Imperfect Gamers Discord Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "Plugin for Discord Webhooks for Imperfect Gamers";

    private static Config? _config;
    private static readonly DiscordService _discordService;

    static IGDiscordPlugin()
    {
        _discordService = new DiscordService();
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
            var serverStatusMessage = _config.ServerStatusMessage;

            if (serverStatusMessage != null)
            {
                Task.Run(async () =>
                {
                    await _discordService.SendServerStatusMessage(serverStatusMessage);
                });
            }
        }
        else
        {
            Util.PrintLog("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }

    private void GetOrCreateConfig()
    {
        var configPath = Path.Join(ModuleDirectory, "Config.json");

        if (!File.Exists(configPath))
        {
            // This is the first time running this plugin, create the config file
            var configData = new Config()
            {
                ServerStatusMessage = new ServerStatusMessageInfo()
                {
                    Type = Constants.MessageType.ServerStatus,
                    Prefix = "### SAMPLE SERVER STATUS MESSAGE PREFIX ###",
                    WebhookUri = "https://discord.com/api/webhooks/###############/#################",
                    MessageInterval = 300
                }
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var serializedConfigData = JsonSerializer.Serialize(configData, jsonOptions);
            File.WriteAllText(configPath, serializedConfigData);

            Util.PrintLog("Created config.json file");

            _config = configData;
        }
        else
        {
            var configData = File.ReadAllText(configPath);

            Util.PrintLog("Read config.json file");

            _config = JsonSerializer.Deserialize<Config>(configData);
        }
    }
}