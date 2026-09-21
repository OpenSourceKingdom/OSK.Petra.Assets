using OSK.Petra.Modules;
using System;
using OSK.Petra.Assets.Events;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

namespace OSK.Petra.Assets;

public abstract class ModuleLoader<TLoadParameters>(IModuleDescriptor descriptor, TLoadParameters parameters)
    : IModuleLoader
    where TLoadParameters: ModuleLoadParameters
{
    #region Variables

    private bool _finalized;
    private bool _disposed;

    protected IModuleDescriptor Descriptor => descriptor;
    protected TLoadParameters Parameters => parameters;

    #endregion

    #region IModuleLoader

    public event Action<ModuleLoadEvent>? LoadEvent;

    public ModuleName ModuleName => descriptor.Name;

    public LoadProgress LoadProgress { get; private set; } = LoadProgress.NotStarted;

    public void FinalizeLoad()
    {
        if (!LoadProgress.IsComplete || _finalized)
        {
            return;
        }

        LoadModule();
        _finalized = true;

        LoadEvent?.Invoke(new ModuleLoadFinalizedEvent(this));
    }

    public void Update(TimeSpan deltaTime)
    {
        if (LoadProgress.State is ProgressState.NotStarted)
        {
            LoadEvent?.Invoke(new ModuleLoadStartedEvent(this));
        }
        else if (LoadProgress.State is not ProgressState.InProgress)
        {
            return;
        }

        LoadProgress = UpdateProgress(deltaTime);
        switch (LoadProgress.State)
        {
            case ProgressState.Complete:
                LoadEvent?.Invoke(new ModuleLoadCompleteEvent(this));
                if (Parameters.LoadCompletionMode is LoadCompletionMode.Immediate)
                {
                    FinalizeLoad();
                }
                break;
            case ProgressState.Failed:
                LoadEvent?.Invoke(new ModuleLoadFailedEvent(this));
                break;
            default:
                LoadEvent?.Invoke(new ModuleLoadProgressEvent(this));
                break;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        DisposeLoader();
        LoadProgress = LoadProgress.IsComplete
            ? LoadProgress
            : new(Out.Error(OutputStatus.Timeout, "Loader disposed"));

        _disposed = true;
    }

    #endregion

    #region Helpers

    protected abstract void DisposeLoader();

    /// <summary>
    /// Called to finalize the load. e.g. you should place the module in the game and allow it to run
    /// </summary>
    protected abstract void LoadModule();

    /// <summary>
    /// Update the loade through another iteration
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    protected abstract LoadProgress UpdateProgress(TimeSpan deltaTime);

    #endregion
}
