namespace OSK.Petra.Assets.Models;

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
