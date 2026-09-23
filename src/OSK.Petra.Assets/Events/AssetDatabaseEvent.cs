using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered with an <see cref="IAssetDatabaseContext"/>
/// </summary>
/// <param name="context">The context that triggered the event</param>
public abstract class AssetDatabaseEvent(IAssetDatabaseContext context)
{
    /// <summary>
    /// The database context that is associated with the triggered event
    /// </summary>
    public IAssetDatabaseContext DatabaseContext => context;
}
