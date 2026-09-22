using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered to provide progress updates to consumers
/// </summary>
/// <param name="context">The context associated with the event</param>
public class ModuleLoadProgressEvent(IModuleLoadContext context)
    : ModuleLoadEvent(context)
{
}
