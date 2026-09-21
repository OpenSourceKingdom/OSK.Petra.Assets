using OSK.Operations.Outputs.Models;
using System;

namespace OSK.Petra.Assets.Models;

public readonly struct LoadProgress
{
    #region Static

    public static readonly LoadProgress Complete = new(1);
    public static readonly LoadProgress NotStarted = new();

    #endregion

    #region Varaibles

    public bool IsComplete => State is ProgressState.Complete;

    public ProgressState State { get; }

    public double Percentage { get; }

    public Output? Error { get; }

    public string Messaage { get; }

    #endregion

    #region Constructors

    public LoadProgress()
    {
        State = ProgressState.NotStarted;
        Messaage = "Not Started";
    }

    public LoadProgress(double percentage, string? message = null)
    {
        Percentage = percentage < 0 
                        ? 0 
                        : percentage > 1 ? 1 : percentage;
        
        State = percentage >= 1 ? ProgressState.Complete : ProgressState.InProgress;
        Messaage = string.IsNullOrWhiteSpace(message) ? $"Loading: {percentage}%" : message;
    }

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
