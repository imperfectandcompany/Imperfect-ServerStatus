using ImperfectServerStatus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperfectServerStatus.Services.Interfaces
{
    public interface IConfigService
    {
        string GetConfigPath(string moduleDirectory, string moduleName);

        void UpdateConfig(Config configData, string configPath);
    }
}
