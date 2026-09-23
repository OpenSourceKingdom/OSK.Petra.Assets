using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered when a module has officially begun to be loaded
/// </summary>
/// <param name="context">The context that is associated with the event</param>
public class ModuleLoadStartedEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
