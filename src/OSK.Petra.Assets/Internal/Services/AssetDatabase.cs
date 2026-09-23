using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Events;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Assets.Internal.Services;

internal class AssetDatabase : IAssetDatabase
{
    #region Variables

    private readonly Dictionary<EntityAssetIdentifier, IEntityDescriptor> _entityDescriptors = [];
    private readonly Dictionary<ModuleAssetIdentifier, IModuleDescriptor> _moduleDescriptors = [];

    #endregion

    #region IAssetDatabase

    public event Action<AssetDatabaseEvent>? DatabaseEvent;

    public void StartInitialization()
    {
        _entityDescriptors.Clear();
        _moduleDescriptors.Clear();
        InitializationProgress = new(0, "Initializing Database...");

        DatabaseEvent?.Invoke(new DatabaseInitializationStartedEvent(this));
    }

    public IModuleDescriptor? GetModule(ModuleAssetIdentifier identifier)
    => _moduleDescriptors.TryGetValue(identifier, out var module) ? module : null;

    public IEntityDescriptor? GetEntity(EntityAssetIdentifier identifier)
        => _entityDescriptors.TryGetValue(identifier, out var entity) ? entity : null;

    public IEnumerable<IModuleDescriptor> GetModules(AssetSearchOptions? searchOptions = null)
    {
        if (InitializationState is not ProgressState.Complete)
        {
            return [];
        }

        var descriptors = searchOptions?.AssetPackageIds is null || searchOptions.Value.AssetPackageIds.Length is 0
            ? _moduleDescriptors.Values
            : _moduleDescriptors.Values.Where(moduleDescriptor => searchOptions.Value.AssetPackageIds.Contains(moduleDescriptor.AssetIdentifier.AssetPackageId));

        if (searchOptions?.TagFilters is { Length: > 0 })
        {
            descriptors = descriptors.Where(descriptor => searchOptions.Value.TagFilters.All(matcher => descriptor.Tags.Any(matcher.Matches)));
        }

        return descriptors;
    }

    public IEnumerable<IEntityDescriptor> GetEntities(AssetSearchOptions? searchOptions = null)
    {
        if (InitializationState is not ProgressState.Complete)
        {
            return [];
        }

        var descriptors = searchOptions?.AssetPackageIds is null || searchOptions.Value.AssetPackageIds.Length is 0
            ? _entityDescriptors.Values
            : _entityDescriptors.Values.Where(moduleDescriptor => searchOptions.Value.AssetPackageIds.Contains(moduleDescriptor.AssetIdentifier.AssetPackageId));

        if (searchOptions?.TagFilters is { Length: > 0 })
        {
            descriptors = descriptors.Where(descriptor => searchOptions.Value.TagFilters.All(matcher => descriptor.Tags.Any(matcher.Matches)));
        }

        return descriptors;
    }

    public ProgressState InitializationState => InitializationProgress.State;

    public LoadProgress InitializationProgress { get; private set; }

    public int TotalEntities => _entityDescriptors.Count;

    public int TotalModules => _moduleDescriptors.Count;

    public void UpdateProgress(float progress, string? message = null)
    {
        if (InitializationState is not ProgressState.InProgress)
        {
            return;
        }

        InitializationProgress = new(progress, message);
    }

    public void AddAssets(IEnumerable<IAssetDescriptor> assetDescriptors)
    {
        if (InitializationState is not ProgressState.InProgress)
        {
            return;
        }

        foreach (var descriptor in assetDescriptors)
        {
            switch (descriptor)
            {
                case IEntityDescriptor entityDescriptor:
                    _entityDescriptors[entityDescriptor.AssetIdentifier] = entityDescriptor;
                    break;
                case IModuleDescriptor moduleDescriptor:
                    _moduleDescriptors[moduleDescriptor.AssetIdentifier] = moduleDescriptor;
                    break;
            }
        }
    }

    public void Succeed()
    {
        InitializationProgress = new(1);

        DatabaseEvent?.Invoke(new DatabaseInitializationCompleteEvent(this));
    }

    public void Fail(Output error)
    {
        _entityDescriptors.Clear();
        _moduleDescriptors.Clear();
        InitializationProgress = new(error);

        DatabaseEvent?.Invoke(new DatabaseInitializationFailedEvent(this, error.StatusCode, error.GetErrorString()));
    }

    #endregion
}
