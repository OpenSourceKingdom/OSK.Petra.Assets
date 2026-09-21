using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Ports;

namespace OSK.Petra.Assets.Internal.Services;

internal class NoOpLoadScreen : ILoadScreen
{
    public void Initialize(ModuleLoadParameters loadParameters, IModuleLoadContext context)
    {
    }
}
