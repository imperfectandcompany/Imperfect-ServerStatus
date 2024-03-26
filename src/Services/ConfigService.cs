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

                Util.PrintLog("Updated config.json file");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating the config: {message}", ex.Message);
            }
        }
    }
}
