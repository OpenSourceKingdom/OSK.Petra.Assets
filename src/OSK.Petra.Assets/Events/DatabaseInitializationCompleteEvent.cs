using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class DatabaseInitializationCompleteEvent(IAssetDatabaseContext context)
    : AssetDatabaseEvent(context)
{
}
