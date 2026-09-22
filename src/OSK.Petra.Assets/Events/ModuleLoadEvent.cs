using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered wtihin <see cref="IModuleLoadContext"/>
/// </summary>
/// <param name="context">The context that triggered the event</param>
public abstract class ModuleLoadEvent(IModuleLoadContext context)
{
    /// <summary>
    /// The context that the event is associated with
    /// </summary>
    public IModuleLoadContext LoadContext => context;
}
