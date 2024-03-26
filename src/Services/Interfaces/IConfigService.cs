using ImperfectServerStatus.Models;

namespace ImperfectServerStatus.Services.Interfaces
{
    public interface IConfigService
    {
        void UpdateConfig(Config configData, string configPath);

        void UpdateConfigToNewVersion(Config configData, string configPath);
    }
}
