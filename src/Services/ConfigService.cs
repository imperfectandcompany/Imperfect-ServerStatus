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

        public Config? LoadConfig(string moduleDirectory)
        {
            var configPath = Path.Join(moduleDirectory, "Config.json");

            if (configPath != null)
            {
                if (!File.Exists(configPath))
                {
                    // This is the first time running this plugin, create the config file
                    _config = CreateConfig(configPath);
                }
                else
                {
                    // Config exists, get it
                    _config = GetConfig(configPath);
                }

                if (_config == null)
                {
                    Util.PrintError($"Something went wrong when retrieving config.json.");
                }
            }

            return _config;
        }

        private Config? CreateConfig(string configPath)
        {
            try
            {
                var configData = new Config()
                {
                    StatusMessageInfo = new StatusMessageInfo()
                    {
                        MessageType = Constants.MessageType.ServerStatus,
                        WebhookUri = "https://discord.com/api/webhooks/###############/#################",
                        MessageInterval = 300
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var serializedConfigData = JsonSerializer.Serialize(configData, jsonOptions);
                File.WriteAllText(configPath, serializedConfigData);

                _config = configData;
                Util.PrintLog(configData.StatusMessageInfo.WebhookUri);

                Util.PrintLog("Created config.json file");
            }
            catch (Exception ex)
            {
                Util.PrintError($"Error creating config.json: {ex}");
            }

            return _config;
        }

        private Config? GetConfig(string configPath)
        {
            try
            {
                var configData = File.ReadAllText(configPath);

                Util.PrintLog("Read config.json file");

                _config = JsonSerializer.Deserialize<Config>(configData);
            }
            catch (Exception ex)
            {
                Util.PrintError($"Error reading config.json: {ex}");
            }

            return _config;
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
