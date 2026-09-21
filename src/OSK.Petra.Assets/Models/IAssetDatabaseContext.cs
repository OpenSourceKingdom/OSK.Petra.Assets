using OSK.Petra.Assets.Events;
using System;

namespace OSK.Petra.Assets.Models;

public interface IAssetDatabaseContext
{
    event Action<AssetDatabaseEvent>? DatabaseEvent;

    ProgressState InitializationState { get; }

    int TotalEntities { get; }

    int TotalModules { get; }
}
