using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class ModuleLoadStartedEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
