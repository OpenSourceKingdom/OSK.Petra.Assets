using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered to inform consumers about progress during the initialization of the database
/// </summary>
/// <param name="context">The context for the triggered event</param>
/// <param name="progress">A percentage value (0-1) of the initialization state of the databse</param>
public class DatabaseInitializationProgressEvent(IAssetDatabaseContext context, LoadProgress progress)
    : AssetDatabaseEvent(context)
{
    /// <summary>
    /// A percentage value (0-1) of the initialization state of the database
    /// </summary>
    public double Progress => progress.Percentage;

    /// <summary>
    /// A status update message
    /// </summary>
    public string Message => progress.Messaage;
}
