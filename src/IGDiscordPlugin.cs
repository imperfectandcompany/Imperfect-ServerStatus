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

    public Config? _config;
    private readonly DiscordService _discordService;
    private readonly ConfigService _configService;

    public IGDiscordPlugin()
    {
        _discordService = new DiscordService();
        _configService = new ConfigService();
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    public override void Load(bool hotReload)
    {
        _config = _configService.LoadConfig(ModuleDirectory);

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
            Util.PrintError("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }
}