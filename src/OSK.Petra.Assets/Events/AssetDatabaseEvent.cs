using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public abstract class AssetDatabaseEvent(IAssetDatabaseContext context)
{
    public IAssetDatabaseContext DatabaseContext => context;
}
