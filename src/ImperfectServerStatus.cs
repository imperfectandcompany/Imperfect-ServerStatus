using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using ImperfectServerStatus.Models;
using ImperfectServerStatus.Models.Discord;
using ImperfectServerStatus.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ImperfectServerStatus;

public partial class ImperfectServerStatus : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Imperfect-ServerStatus";
    public override string ModuleVersion => "1.5.0";
    public override string ModuleAuthor => "Imperfect Gamers - raz";
    public override string ModuleDescription => "A Discord server status plugin for Imperfect Gamers";

    // Define the FakeConVar for the IP address override
    private FakeConVar<string> _imperfectStatusIp = new(
        "imperfect_status_ip", // The ConVar name used on the command line
        "Specifies the IP address override for ImperfectServerStatus plugin.",
        "", // Default value (empty => fallback to config file)
        ConVarFlags.FCVAR_NONE
    );

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
        ILogger<ImperfectServerStatus> logger, 
        string configPath, 
        WebhookMessage webhookMessage)
    {
        _configService = configService;
        _discordService = discordService;
        _logger = logger;
        ConfigPath = configPath;
        _webhookMessage = webhookMessage;
    }

    public override void Load(bool hotReload)
    {
        // Register the FakeConVar so the engine sees it.
        RegisterFakeConVars(GetType());

        if (Config != null)
        {
            _statusData.Timestamp = DateTime.Now;
            _webhookMessage = _discordService.CreateWebhookMessage(Config.StatusInfo, _statusData);

            RegisterListener<Listeners.OnHostNameChanged>(OnHostNameChanged);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
        }
        else
        {
            _logger.LogInformation(
                "The config file did not load correctly. Please check that there is a {ModuleName}.json file in the CounterStrikeSharp config directory.",
                ModuleName);
        }
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
    }

    public void OnConfigParsed(Config config)
    {
        string assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "";
        ConfigPath = $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/configs/plugins/{assemblyName}/{assemblyName}.json";

        if (config.Version < Config.Version)
        {
            _logger.LogWarning(
                "The config version does not match current version: Expected: {0} | Current: {1}",
                Config.Version, config.Version);
            // TODO: Update config file to current version.
        }

        if (string.IsNullOrEmpty(config.ServerIp))
        {
            _logger.LogWarning(
                "Server IP is missing from config. Set a value to create connection links and display properly."
            );
        }

        // Apply config
        Config = config;

        // Check if the ConVar is set. If it has a non-empty value, override the IP:
        string overrideIp = _imperfectStatusIp.Value?.Trim() ?? "";
        if (!string.IsNullOrEmpty(overrideIp))
        {
            Config.ServerIp = overrideIp; // override the config with the ConVar
            _logger.LogInformation(
                "[ImperfectServerStatus] Overriding config.ServerIp with '{OverrideIp}' from imperfect_status_ip ConVar.",
                overrideIp
            );
        }

        // Pass the final IP into the status data if not null
        if (!string.IsNullOrEmpty(Config.ServerIp))
        {
            _statusData.IpAddress = Config.ServerIp;
        }
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
}
