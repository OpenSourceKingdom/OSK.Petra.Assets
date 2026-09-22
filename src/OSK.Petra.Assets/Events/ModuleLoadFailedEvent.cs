using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that indicates some kind of failure was encountered while loading a particular module. See the <see cref="IModuleLoadContext"/> for more details.
/// </summary>
/// <param name="context">The context that triggered the event</param>
public class ModuleLoadFailedEvent(IModuleLoadContext context): ModuleLoadEvent(context)
{
}
