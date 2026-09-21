using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Events;

public class DatabaseInitializationFailedEvent(IAssetDatabaseContext context, OutputCode statusCode, string errorMessage)
    : AssetDatabaseEvent(context)
{
    public OutputCode StatusCode => statusCode;

    public string ErrorMessage => errorMessage;
}
