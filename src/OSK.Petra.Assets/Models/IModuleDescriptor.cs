using System;

namespace OSK.Petra.Assets.Models;

public interface IModuleDescriptor: IAssetDescriptor
{
    ModuleAssetIdentifier AssetIdentifier { get; }

    Type GetModuleLoaderType();
}
