using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class ModuleLoadFinalizedEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
