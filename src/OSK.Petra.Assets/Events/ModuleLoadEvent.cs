using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public abstract class ModuleLoadEvent(IModuleLoadContext context)
{
    public IModuleLoadContext LoadContext => context;
}
