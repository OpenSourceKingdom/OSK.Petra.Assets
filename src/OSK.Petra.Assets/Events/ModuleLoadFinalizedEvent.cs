using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that signifies that a module has been completely finalized and is ready for usage within an application
/// </summary>
/// <param name="context">The context associated with the event</param>
public class ModuleLoadFinalizedEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
