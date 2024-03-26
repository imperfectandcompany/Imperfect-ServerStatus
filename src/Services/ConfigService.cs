using CounterStrikeSharp.API.Core;
using ImperfectServerStatus.Models;
using ImperfectServerStatus.Services.Interfaces;
using ImperfectServerStatus.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ImperfectServerStatus.Services
{
    public class ConfigService : IConfigService
    {
        private readonly ILogger<ImperfectServerStatus> _logger;

        public ConfigService(ILogger<ImperfectServerStatus> logger)
        {
            _logger = logger;
        }

        public void UpdateConfig(Config configData, string configPath)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var serializedConfigData = JsonSerializer.Serialize(configData, jsonOptions);
                File.WriteAllText(configPath, serializedConfigData);

                Util.PrintLog("Updated config file");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating the config: {message}", ex.Message);
            }
        }

        public void UpdateConfigToNewVersion(Config config, string cfgPath)
        { 
            /// TODO: This method should do some checking
            ///     Did the shape of the config change? We need to adjust
            ///         This just updates the version and kind of goes against the point 
            ///         of a config version, if things are taken out or added, we need to 
            ///         know what things changed and adjust the new config to the new shape

            // get newest config version
            var newCfgVersion = new Config().Version;

            // loaded config is up to date
            if (config.Version == newCfgVersion)
                return;

            // update the version
            config.Version = newCfgVersion;

            // serialize the updated config back to json
            UpdateConfig(config, cfgPath);

            Util.PrintLog("Updated config version");
        }
    }
}
