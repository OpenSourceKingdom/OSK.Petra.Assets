using OSK.Petra.Assets.Events;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Ports;
using OSK.Petra.Modules;

namespace OSK.Petra.Assets.UnitTests._Helpers;

public class TestLoader : IModuleLoader
{
    public TestLoader(ModuleLoadParameters parameters, IModuleDescriptor descriptor)
    {

    }

    public ModuleName ModuleName => throw new NotImplementedException();

    public LoadProgress LoadProgress => throw new NotImplementedException();

    public event Action<ModuleLoadEvent>? LoadEvent;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void FinalizeLoad()
    {
        throw new NotImplementedException();
    }

    public void Update(TimeSpan deltaTime)
    {
        throw new NotImplementedException();
    }
}
