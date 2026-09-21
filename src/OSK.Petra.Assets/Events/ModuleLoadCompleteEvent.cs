using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class ModuleLoadCompleteEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
