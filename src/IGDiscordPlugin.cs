using CounterStrikeSharp.API.Core;
using IGDiscord.Models;
using System.Text.Json;

namespace IGDiscord;

public class IGDiscordPlugin : BasePlugin
{
    public override string ModuleName => "Imperfect Gamers Discord Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "Plugin for Discord Webhooks for Imperfect Gamers";

    private static Config? _config;

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    public override void Load(bool hotReload)
    {
        GetOrCreateConfig();
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