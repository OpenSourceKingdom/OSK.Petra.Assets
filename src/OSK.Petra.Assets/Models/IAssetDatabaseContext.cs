using OSK.Petra.Assets.Events;
using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// A contextual data structure that provides insights into the database 
/// </summary>
public interface IAssetDatabaseContext
{
    /// <summary>
    /// An event that is triggered if something special happens within the database
    /// </summary>
    event Action<AssetDatabaseEvent>? DatabaseEvent;

    /// <summary>
    /// The current state for the initialization of the database. This should be validated as complete before using the related asset APIs
    /// </summary>
    ProgressState InitializationState { get; }

    /// <summary>
    /// The total number of unique entities loaded into the database
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This should be considered as valid information only when the database initialization state is complete</item>
    /// </list>
    /// </remarks>
    int TotalEntities { get; }

    /// <summary>
    /// The total number of unique modules loaded into the database
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This should be considered as valid information only when the database initialization state is complete</item>
    /// </list>
    /// </remarks>
    int TotalModules { get; }
}
