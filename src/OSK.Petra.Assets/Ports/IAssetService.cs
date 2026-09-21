using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Assets.Ports;

public interface IAssetService
{
    IAssetDatabaseContext DatabaseContext { get; }

    Task<Output> InitializeAsync(AssetServiceOptions? options = null, CancellationToken cancellationToken = default);

    void Update(TimeSpan deltaTime);

    ValueTask<Output<TEntity>> InstantiateAsync<TEntity, TTransform>(InstantiationParameters<TTransform> parameters, CancellationToken cancellationToken = default)
        where TEntity: class
        where TTransform: ITransform;

    IModuleLoadContext? LoadModule<TLoadParameters>(TLoadParameters parameters)
        where TLoadParameters: ModuleLoadParameters;

    IModuleDescriptor? GetModuleDescriptor(ModuleAssetIdentifier identifier);

    IEntityDescriptor? GetEntityDescriptor(EntityAssetIdentifier identifier);

    IEnumerable<IModuleDescriptor> GetModuleDescriptors(AssetSearchOptions? searchOptions = null);

    IEnumerable<IEntityDescriptor> GetEntityDescriptors(AssetSearchOptions? searchOptions = null);
}
