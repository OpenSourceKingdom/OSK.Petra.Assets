using OSK.Petra.Assets.Events;
using OSK.Petra.Modules;
using System;

namespace OSK.Petra.Assets.Models;

public interface IModuleLoadContext: IDisposable
{
    event Action<ModuleLoadEvent>? LoadEvent;

    ModuleName ModuleName { get; }

    LoadProgress LoadProgress { get; }

    void FinalizeLoad();
}
