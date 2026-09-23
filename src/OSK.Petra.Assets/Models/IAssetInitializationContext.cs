using OSK.Operations.Outputs.Models;
using System.Collections.Generic;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// A database context that is capable of providing asset and progress related updates to the asset system
/// </summary>
public interface IAssetInitializationContext
{
    /// <summary>
    /// Sets the current initialization progress of the database
    /// </summary>
    /// <param name="progress">The current progress for the initialization of the database. This should be a value between 0-1/</param>
    /// <param name="message">An optional message that can be used to inform application users of the current state prgoress</param>
    void UpdateProgress(float progress, string? message = null);

    /// <summary>
    /// Adds the provided assets to the database
    /// </summary>
    /// <param name="assetDescriptors">The collection of descriptors that the database should register</param>
    void AddAssets(IEnumerable<IAssetDescriptor> assetDescriptors);

    /// <summary>
    /// Informs the asset system that the database has been fully initialized and no other work is needed
    /// </summary>
    void Succeed();

    /// <summary>
    /// Informs the asset system that an error was encountered during the initialization process that should beed addressed before reinitializing
    /// </summary>
    /// <param name="error">The specific error that failed the initialization</param>
    void Fail(Output error);
}
