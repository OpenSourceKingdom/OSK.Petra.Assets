using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered when a database initialziation process has started
/// </summary>
/// <param name="context"></param>
public class DatabaseInitializationStartedEvent(IAssetDatabaseContext context)
    : AssetDatabaseEvent(context)
{
}
