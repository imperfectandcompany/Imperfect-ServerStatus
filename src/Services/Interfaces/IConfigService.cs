using ImperfectServerStatus.Models;

namespace ImperfectServerStatus.Services.Interfaces
{
    public interface IConfigService
    {
        string GetConfigPath(string moduleDirectory, string moduleName);

        void UpdateConfig(Config configData, string configPath);
    }
}
