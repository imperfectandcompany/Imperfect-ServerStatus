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
    private DiscordService _discordService;
    private ConfigService _configService;

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    public override void Load(bool hotReload)
    {
        _configService = new ConfigService(ModuleDirectory);
        _discordService = new DiscordService(_configService);

        _config = _configService.LoadConfig(ModuleDirectory);

        if (_config != null)
        {
            Task.Run(async () =>
            {
                await _discordService.SendServerStatusMessage(_config);
            });
        }
        else
        {
            Util.PrintError("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }
}