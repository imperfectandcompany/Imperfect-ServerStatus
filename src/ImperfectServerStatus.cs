using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using ImperfectServerStatus.Models;
using ImperfectServerStatus.Models.Discord;
using ImperfectServerStatus.Services.Interfaces;
using ImperfectServerStatus.Utils;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

namespace ImperfectServerStatus;

public partial class ImperfectServerStatus : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "ImperfectServerStatus";
    public override string ModuleVersion => "1.3.0";
    public override string ModuleAuthor => "raz";
    public override string ModuleDescription => "A Discord server status plugin for Imperfect Gamers";

    public Config Config { get; set; } = new();
    public string ConfigPath;

    public StatusData _statusData = new();
    private WebhookMessage _webhookMessage;

    private readonly IConfigService _configService;
    private readonly IDiscordService _discordService;
    private readonly ILogger<ImperfectServerStatus> _logger;

    public ImperfectServerStatus(
        IConfigService configService,
        IDiscordService discordService,
        ILogger<ImperfectServerStatus> logger)
    {
        _configService = configService;
        _discordService = discordService;
        _logger = logger;
    }

    public void OnHostNameChanged(string hostName)
    {
        _statusData.ServerOnline = true;
        _statusData.ServerName = hostName;
        _statusData.MapName = Server.MapName;

        if (string.IsNullOrEmpty(Config.StatusInfo.MessageId))
        {
            CreateDiscordStatusMessage();
        }

        UpdateDiscordStatusMessage();
    }

    public void OnMapStart(string mapName)
    {
        _statusData.ServerOnline = true;
        _statusData.MapName = mapName;


        UpdateDiscordStatusMessage();
    }

    public override void Load(bool hotReload)
    {
        if (Config != null)
        {
            _statusData.Timestamp = DateTime.Now;

            _webhookMessage = _discordService.CreateWebhookMessage(Config.StatusInfo, _statusData);

            RegisterListener<Listeners.OnHostNameChanged>(OnHostNameChanged);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
        }
        else
        {
            _logger.LogInformation("The config file did not load correctly. Please check that there is a {ModuleName}.json file in the CounterStrikeSharp config directory.", ModuleName);
        };
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    private void CreateDiscordStatusMessage()
    {
        // Send initial message
        Task.Run(async () =>
        {
            var messageId = await _discordService.CreateStatusMessageAsync(Config.StatusInfo, _webhookMessage);

            if (!string.IsNullOrEmpty(messageId))
            {
                Config.StatusInfo.MessageId = messageId;

                _configService.UpdateConfig(Config, ConfigPath);
            }
            else
            {
                _logger.LogError("Something went wrong getting a response when sending message.");
            }
        });
    }

    private void UpdateDiscordStatusMessage()
    {
        // Update the message
        Task.Run(async () =>
        {
            _statusData.Timestamp = DateTime.Now;

            WebhookMessage updatedWebhookMessage = _discordService.UpdateWebhookMessage(_webhookMessage, _statusData);

            await _discordService.UpdateStatusMessageAsync(Config.StatusInfo, updatedWebhookMessage);
        });
    }

    public void OnConfigParsed(Config config)
    {
        ConfigPath = _configService.GetConfigPath(ModuleDirectory, ModuleName);

        if (config.Version < Config.Version)
        {
            _logger.LogWarning("The config version does not match current version: Expected: {0} | Current: {1}", Config.Version, config.Version);

            // TODO: Update config file to current version.
        }

        if (string.IsNullOrEmpty(config.ServerIp))
        {
            _logger.LogWarning("Server IP is missing from config. Set a value to avoid issues with connection links.");
        }
        else
        {
            _statusData.IpAddress = config.ServerIp;
        }

        Config = config;
    }
}