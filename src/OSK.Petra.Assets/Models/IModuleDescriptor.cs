using System;
using OSK.Petra.Assets.Ports;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes an entire module that can be used within the asset system
/// </summary>
public interface IModuleDescriptor: IAssetDescriptor
{
    /// <summary>
    /// The unique identifier for the module
    /// </summary>
    ModuleAssetIdentifier AssetIdentifier { get; }

    /// <summary>
    /// Gets the loader type that should be used to load and initialize the module
    /// </summary>
    /// <returns>The type of <see cref="IModuleLoader"/> that should be used to load the module</returns>
    Type GetModuleLoaderType();
}
