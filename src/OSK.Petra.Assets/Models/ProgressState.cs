namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes the state of a load progression
/// </summary>
public enum ProgressState
{
    /// <summary>
    /// The progression has not yet started
    /// </summary>
    NotStarted,

    /// <summary>
    /// The progression is being updated
    /// </summary>
    InProgress,

    /// <summary>
    /// The progression has finished successfully
    /// </summary>
    Complete,

    /// <summary>
    /// The progression has finished after encountering an error
    /// </summary>
    Failed
}
