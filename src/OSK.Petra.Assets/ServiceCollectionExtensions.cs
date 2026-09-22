using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSK.Petra.Assets.Internal;
using OSK.Petra.Assets.Internal.Services;
using OSK.Petra.Assets.Ports;

namespace OSK.Petra.Assets;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds all the required asset system code to the DI
        /// </summary>
        /// <returns>The services for chaining</returns>
        public IServiceCollection AddAssets()
        {
            services.TryAddSingleton<IAssetService, AssetService>();
            services.TryAddSingleton<IAssetDatabase, AssetDatabase>();

            // Integrations can add their own load screens, if needed, otherwise we'll just do nothing with it
            services.TryAddSingleton<ILoadScreen, NoOpLoadScreen>();

            return services;
        }
    }
}
