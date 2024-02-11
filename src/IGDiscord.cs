using CounterStrikeSharp.API.Core;
using IGDiscord.Models.MessageInfo;
using IGDiscord.Models;
using IGDiscord.Services;
using IGDiscord.Utils;
using System.Text.Json;
using IGDiscord.Services.Interfaces;

namespace IGDiscord;

public class IGDiscord : BasePlugin
{
    public override string ModuleName => "IGDiscord";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "A Discord webhook plugin for Imperfect Gamers";

    public Config? _config;
    private readonly IConfigService _configService;
    private readonly IDiscordService _discordService;

    public IGDiscord(
        IConfigService configService,
        IDiscordService discordService)
    {
        _configService = configService;
        _discordService = discordService;
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