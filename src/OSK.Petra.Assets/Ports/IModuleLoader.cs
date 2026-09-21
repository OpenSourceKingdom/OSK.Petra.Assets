using OSK.Petra.Assets.Models;
using System;

namespace OSK.Petra.Assets.Ports;

public interface IModuleLoader: IModuleLoadContext
{
    void Update(TimeSpan deltaTime);
}
