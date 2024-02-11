using IGDiscord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.Services.Interfaces
{
    public interface IConfigService
    {
        string GetConfigPath(string moduleDirectory, string moduleName);

        Config? LoadConfig(string moduleDirectory);

        void UpdateConfig(Config configData, string configPath);
    }
}
