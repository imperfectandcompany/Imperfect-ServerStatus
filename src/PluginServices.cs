namespace ImperfectServerStatus
{
    using CounterStrikeSharp.API.Core;
    using global::ImperfectServerStatus.Services;
    using global::ImperfectServerStatus.Services.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public class PluginServices : IPluginServiceCollection<ImperfectServerStatus>
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDiscordService, DiscordService>();
            serviceCollection.AddTransient<IConfigService, ConfigService>();
        }
    }
}