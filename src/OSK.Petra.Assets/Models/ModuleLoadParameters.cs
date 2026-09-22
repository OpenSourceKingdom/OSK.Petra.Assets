using OSK.Petra.Modules;
using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes the set of parameters required for loading a module
/// </summary>
/// <param name="moduleName">The name of the module to load</param>
/// <param name="assetPackageId">The package the module belongs to</param>
public class ModuleLoadParameters(ModuleName moduleName, Guid? assetPackageId = null)
{
    /// <summary>
    /// The unique identifier for the module being loaded
    /// </summary>
    public ModuleAssetIdentifier ModuleIdentifier { get; } = new(assetPackageId ?? AssetIdentifiers.DefaultPackageId, moduleName.Name);

    /// <summary>
    /// The behavior the module will have when being loaded
    /// </summary>
    public ModuleLoadBehavior LoadBehavior { get; set; }

    /// <summary>
    /// The behvaior for finalizing the module load
    /// </summary>
    public ModuleFinalizationMode FinalizationMode { get; set; }

    /// <summary>
    /// The parent module the loaded module will be associated with
    /// </summary>
    public IModule? ParentModule { get; set; }
}
