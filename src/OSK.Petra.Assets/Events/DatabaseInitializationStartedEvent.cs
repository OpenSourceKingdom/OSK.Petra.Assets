using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class DatabaseInitializationStartedEvent(IAssetDatabaseContext context)
    : AssetDatabaseEvent(context)
{
}
