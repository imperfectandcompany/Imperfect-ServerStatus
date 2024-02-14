using IGDiscord.Models.MessageInfo;
using IGDiscord.Models;
using IGDiscord.Utils;
using IGDiscord.Services.Interfaces;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API;
using IGDiscord.Models.Discord;

namespace IGDiscord;

public partial class IGDiscord : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "IGDiscord";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "A Discord webhook plugin for Imperfect Gamers";

    public Config Config { get; set; }

    public Config? _config;
    public string ConfigPath;
    public StatusData _statusData = new();
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
            UpdateStatusData();

            WebhookMessage initialWebhookMessage = _discordService.CreateWebhookMessage(Config.StatusMessageInfo, _statusData);

            if (string.IsNullOrEmpty(Config.StatusMessageInfo.MessageId))
            {
                // Send initial message
                Task.Run(async () =>
                {
                    var messageId = await _discordService.SendInitialStatusMessage(Config.StatusMessageInfo, initialWebhookMessage);

                    if (messageId != null)
                    {
                        Config.StatusMessageInfo.MessageId = messageId;

                        _configService.UpdateConfig(Config, ConfigPath);
                    }
                    else
                    {
                        Util.PrintError("Something went wrong getting a reponse when sending message.");
                    }
                });
            }

            // Update the message
            Task.Run(async () =>
            {
                var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(Config.StatusMessageInfo.MessageInterval));
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    Util.PrintLog("Updating status message");

                    UpdateStatusData();

                    WebhookMessage updatedWebhookMessage = _discordService.UpdateWebhookMessage(initialWebhookMessage, _statusData);

                    await _discordService.UpdateStatusMessage(Config.StatusMessageInfo, updatedWebhookMessage);
                }
            });
        }
        else
        {
            _logger.LogInformation("The config file did not load correctly. Please check that there is a config.json file in the plugin directory.");
        };
    }

    private void UpdateStatusData()
    {
        Server.NextFrame(() =>
        {
            StatusData.MapName = NativeAPI.GetMapName();
            StatusData.Timestamp = DateTime.Now;
        });
    }

    public void OnConfigParsed(Config config)
    {
        ConfigPath = _configService.GetConfigPath(ModuleDirectory, ModuleName);

        var configExists = File.Exists(ConfigPath);

        if (configExists is false)
        {
            Util.PrintLog($"Creating {ModuleName}.json for the first time. ");

            config = new Config()
            {
                StatusInfo = new StatusMessageInfo()
                {
                    ServerName = "",
                    IpAddress = "",
                    WebhookUri = "",
                    MessageInterval = 300
                }
            };
        }

        Config = config;
    }
}