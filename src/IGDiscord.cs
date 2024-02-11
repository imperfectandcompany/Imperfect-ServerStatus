using IGDiscord.Models.MessageInfo;
using IGDiscord.Models;
using IGDiscord.Utils;
using CounterStrikeSharp.API.Core;
using IGDiscord.Services.Interfaces;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Modules.Config;

namespace IGDiscord;

public partial class IGDiscord : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "IGDiscord";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "A Discord webhook plugin for Imperfect Gamers";

    public Config Config { get; set; }

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
                await _discordService.SendStatusMessage(_config);
            });
        }
        else
        {
            Util.PrintError("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }

    public void OnConfigParsed(Config config)
    {
        if (config.ServerStatusMessage == null)
        {
            config.ServerStatusMessage = new StatusMessageInfo()
            {
                MessageType = Constants.MessageType.ServerStatus,
                Prefix = "### SAMPLE SERVER STATUS MESSAGE PREFIX ###",
                WebhookUri = "https://discord.com/api/webhooks/###############/#################",
                MessageInterval = 300
            };
        }

        ConfigManager.Load<Config>(ModuleName);

        Config = config;
    }
}