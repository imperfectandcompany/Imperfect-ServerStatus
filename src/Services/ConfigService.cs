using IGDiscord.Models.MessageInfo;
using IGDiscord.Models;
using IGDiscord.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IGDiscord.Services.Interfaces;

namespace IGDiscord.Services
{
    public class ConfigService : IConfigService
    {
        private Config? _config { get; set; }

        public string GetConfigPath(string moduleDirectory, string moduleName)
        {
            string? parentDirectory = Directory.GetParent(path: Directory.GetParent(moduleDirectory).FullName)?.FullName;

            if (parentDirectory != null)
            {
                var configDir = Path.Combine(parentDirectory, $"configs/plugins/{moduleName}");

                var configPath = Path.Combine(configDir, $"{moduleName}.json");

                return configPath;

            }
            else
            {
                Util.PrintError("Error getting config path");
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

                _config = configData;

                Util.PrintLog("Updated config.json file");
            }
            catch (Exception ex)
            {
                Util.PrintError($"Error updaating the config: {ex}");
            }
        }
    }
}
