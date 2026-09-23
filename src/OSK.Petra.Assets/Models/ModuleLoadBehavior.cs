namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes the behavior of the loaded module once it has finalized
/// </summary>
public enum ModuleLoadBehavior
{
    /// <summary>
    /// Loads the new assets and scene in addition to any existing assets; i.e. the currently active scene will remain loaded
    /// </summary>
    Additive,

    /// <summary>
    /// The currently running or active scene will be removed and replaced with the new assets and scene
    /// </summary>
    Replace
}
