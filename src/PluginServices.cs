namespace IGDiscord
{
    using global::IGDiscord.Services;
    using global::IGDiscord.Services.Interfaces;
    using CounterStrikeSharp.API.Core;
    using Microsoft.Extensions.DependencyInjection;

    public class PluginServices : IPluginServiceCollection<IGDiscord>
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDiscordService, DiscordService>();
            serviceCollection.AddTransient<IConfigService, ConfigService>();
        }
    }
}