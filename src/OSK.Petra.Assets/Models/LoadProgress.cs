using OSK.Operations.Outputs.Models;
using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents the progression of a load
/// </summary>
public readonly struct LoadProgress
{
    #region Static

    /// <summary>
    /// A load progress that is in a complete state
    /// </summary>
    public static readonly LoadProgress Complete = new(1);

    /// <summary>
    /// A load progress that is in a state where it has not yet started
    /// </summary>
    public static readonly LoadProgress NotStarted = new();

    #endregion

    #region Varaibles

    /// <summary>
    /// Describes if the progress of the load is complete
    /// </summary>
    public bool IsComplete => State is ProgressState.Complete;

    /// <summary>
    /// The state the load progress describes
    /// </summary>
    public ProgressState State { get; }

    /// <summary>
    /// The percentage of the load progress e.g. 0-1
    /// </summary>
    public double Percentage { get; }

    /// <summary>
    /// An error output, conditionally set if the progress has errored
    /// </summary>
    public Output? Error { get; }

    /// <summary>
    /// A message that describes the load progression
    /// </summary>
    public string Messaage { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a load progress that describes a not started state
    /// </summary>
    public LoadProgress()
    {
        State = ProgressState.NotStarted;
        Messaage = "Not Started";
    }

    /// <summary>
    /// Creates a load progress that describes an in progress state. If the progress is >= 1, the progression is considered complete
    /// </summary>
    /// <param name="percentage">The load progress percentage (e.g. 0-1)</param>
    /// <param name="message">A message that describes the load progression</param>
    public LoadProgress(double percentage, string? message = null)
    {
        Percentage = percentage < 0 
                        ? 0 
                        : percentage > 1 ? 1 : percentage;
        
        State = percentage >= 1 ? ProgressState.Complete : ProgressState.InProgress;
        Messaage = string.IsNullOrWhiteSpace(message) ? $"Loading: {percentage}%" : message;
    }

    /// <summary>
    /// Creates a load progress that describes an error
    /// </summary>
    /// <param name="error">The error that the progress will refer to</param>
    /// <exception cref="InvalidOperationException">The error output can not be successful</exception>
    public LoadProgress(Output error)
    {
        if (error.IsSuccessful)
        {
            throw new InvalidOperationException("Unable to create a load progress with an error output if that output is successful.");
        }

        Error = error;
        Percentage = -1;
        State = ProgressState.Failed;
        Messaage = error.GetErrorString();
    }

    #endregion
}
