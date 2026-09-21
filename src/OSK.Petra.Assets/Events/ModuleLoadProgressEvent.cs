using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class ModuleLoadProgressEvent(IModuleLoadContext context)
    : ModuleLoadEvent(context)
{
}
