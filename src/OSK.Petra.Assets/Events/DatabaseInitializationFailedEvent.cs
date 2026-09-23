using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

/// <summary>
/// An event that is triggered if some kind of error occurred during the initialization of a database. If this is triggered, it should be assumed that the database is not in a usable state and should be reinitialized
/// </summary>
/// <param name="context">The context for the triggered event</param>
/// <param name="statusCode">The error status</param>
/// <param name="errorMessage">An error message for the status</param>
public class DatabaseInitializationFailedEvent(IAssetDatabaseContext context, OutputCode statusCode, string errorMessage)
    : AssetDatabaseEvent(context)
{
    /// <summary>
    /// The output code for the error
    /// </summary>
    public OutputCode StatusCode => statusCode;

    /// <summary>
    /// The error message that describes the failure
    /// </summary>
    public string ErrorMessage => errorMessage;
}
