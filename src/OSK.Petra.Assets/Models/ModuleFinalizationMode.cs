namespace OSK.Petra.Assets.Models;

/// <summary>
/// The different modes for finalizing module load operations
/// </summary>
public enum ModuleFinalizationMode
{
    /// <summary>
    /// The module asset is immediately loaded to the root as soon as it is completed
    /// </summary>
    Immediate,

    /// <summary>
    /// The module asset will wait for any user input before being loaded to the root
    /// </summary>
    AnyUserInput,

    /// <summary>
    /// The module asset will wait for a custom strategy to complete before being loaded to the root; i.e. the game will determine when to complete the load.
    /// This is performed by calling <see cref="IModuleLoadContext.FinalizeLoad"/>
    /// </summary>
    Custom
}
