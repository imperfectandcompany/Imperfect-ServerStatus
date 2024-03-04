using IGDiscord.Models;
using IGDiscord.Utils;
using System.Text.Json;
using IGDiscord.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace IGDiscord.Services
{
    public class ConfigService : IConfigService
    {
        private readonly ILogger<IGDiscord> _logger;

        public ConfigService(ILogger<IGDiscord> logger)
        {
            _logger = logger;
        }

        public string GetConfigPath(string moduleDirectory, string moduleName)
        {
            string parentDirectory = string.Empty;

            var moduleDirectoryParent = Directory.GetParent(moduleDirectory);
            if (moduleDirectoryParent != null)
            {
                var parentOfModuleDirectoryParent = Directory.GetParent(path: moduleDirectoryParent.FullName);
                if (parentOfModuleDirectoryParent != null)
                {
                    parentDirectory = parentOfModuleDirectoryParent.FullName;
                }
            }

            if (!string.IsNullOrEmpty(parentDirectory))
            {
                var configDir = Path.Combine(parentDirectory, $"configs/plugins/{moduleName}");

                var configPath = Path.Combine(configDir, $"{moduleName}.json");

                return configPath;

            }
            else
            {
                _logger.LogError("Error getting config path");
                return string.Empty;
            }
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
