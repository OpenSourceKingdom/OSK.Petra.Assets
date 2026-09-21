using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using System.Collections.Generic;

namespace OSK.Petra.Assets.Internal;

internal interface IAssetDatabase: IAssetDatabaseContext, IAssetInitializationContext
{
    void StartInitialization();

    IModuleDescriptor? GetModule(ModuleAssetIdentifier identifier);

    IEntityDescriptor? GetEntity(EntityAssetIdentifier identifier);

    IEnumerable<IEntityDescriptor> GetEntities(AssetSearchOptions? searchOptions = null);

    IEnumerable<IModuleDescriptor> GetModules(AssetSearchOptions? searchOptions = null);
}
