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
    private readonly ILogger<IGDiscord> _logger;

    public IGDiscord(
        IConfigService configService,
        IDiscordService discordService,
        ILogger<IGDiscord> logger)
    {
        _configService = configService;
        _discordService = discordService;
        _logger = logger;
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    public override void Load(bool hotReload)
    {
        if (Config != null)
        {
            Task.Run(async () =>
            {
                await _discordService.SendStatusMessage(Config);
            });
        }
        else
        {
            _logger.LogInformation("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }

    public void OnConfigParsed(Config config)
    {
        if (config.StatusMessageInfo == null)
        {
            config.StatusMessageInfo = new StatusMessageInfo()
            {
                MessageType = Constants.MessageType.ServerStatus,
                WebhookUri = "https://discord.com/api/webhooks/###############/#################",
                MessageInterval = 300
            };
        }

        Config = config;
    }
}