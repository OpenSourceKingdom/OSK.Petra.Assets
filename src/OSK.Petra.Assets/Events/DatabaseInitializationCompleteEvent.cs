using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered when a datbase has been fully initialized and is ready for usage
/// </summary>
/// <param name="context">The context for the event</param>
public class DatabaseInitializationCompleteEvent(IAssetDatabaseContext context)
    : AssetDatabaseEvent(context)
{
}
