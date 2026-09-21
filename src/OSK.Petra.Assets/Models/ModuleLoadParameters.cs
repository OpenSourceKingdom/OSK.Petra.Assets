using OSK.Petra.Modules;
using System;

namespace OSK.Petra.Assets.Models;

public class ModuleLoadParameters(ModuleName moduleName, Guid? assetPackageId = null)
{
    public ModuleAssetIdentifier ModuleIdentifier { get; } = new(assetPackageId ?? AssetIdentifiers.DefaultPackageId, moduleName.Name);

    public ModuleLoadBehavior LoadBehavior { get; set; }

    public LoadCompletionMode LoadCompletionMode { get; set; }

    public IModule? ParentModule { get; set; }
}
