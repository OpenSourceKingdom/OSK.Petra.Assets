using OSK.Petra.Modules;
using System;
using OSK.Petra.Assets.Events;

namespace OSK.Petra.Assets.Models;

public interface IModuleLoadContext: IDisposable
{
    event Action<ModuleLoadEvent>? LoadEvent;

    ModuleName ModuleName { get; }

    LoadProgress LoadProgress { get; }

    void FinalizeLoad();
}
