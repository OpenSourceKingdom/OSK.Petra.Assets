using OSK.Petra.Assets.Events;
using OSK.Petra.Modules;
using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Provides insights and details into the state of a module load
/// </summary>
public interface IModuleLoadContext: IDisposable
{
    /// <summary>
    /// Events that are triggered throughout the load process
    /// </summary>
    event Action<ModuleLoadEvent>? LoadEvent;

    /// <summary>
    /// The unique module name being loaded
    /// </summary>
    ModuleName ModuleName { get; }

    /// <summary>
    /// The current progress of the load
    /// </summary>
    LoadProgress LoadProgress { get; }

    /// <summary>
    /// Finalizes the load of the module, if the load progress state is <see cref="ProgressState.Complete"/>
    /// </summary>
    void FinalizeLoad();
}
