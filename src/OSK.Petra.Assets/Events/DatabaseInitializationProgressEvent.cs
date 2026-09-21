using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class DatabaseInitializationProgressEvent(IAssetDatabaseContext context, LoadProgress progress)
    : AssetDatabaseEvent(context)
{
    public double Progress => progress.Percentage;

    public string Message => progress.Messaage;
}
