using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class ModuleLoadFailedEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
