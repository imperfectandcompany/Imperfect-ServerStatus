namespace IGDiscord
{
    using IGDiscord.Services;
    using IGDiscord.Services.Interfaces;
    using CounterStrikeSharp.API.Core;
    using Microsoft.Extensions.DependencyInjection;

    public class PluginServices : IPluginServiceCollection<IGDiscordPlugin>
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDiscordService, DiscordService>();
            serviceCollection.AddTransient<IConfigService, ConfigService>();
        }
    }
}