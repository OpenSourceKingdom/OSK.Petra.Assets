using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered when a module loader has completed loading a module and is ready to be finalized
/// </summary>
/// <param name="context">The context that triggered this event</param>
public class ModuleLoadCompleteEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
