using Microsoft.Extensions.DependencyInjection;
using OSK.Expressions.Invoker;
using OSK.Expressions.Invoker.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Internal.Models;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using OSK.Petra.Assets.Ports;
using OSK.Petra.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Assets.Internal.Services;

internal class AssetService(IAssetManager assetManager, ILoadScreen loadScreen, IAssetDatabase database, IServiceProvider services) : IAssetService
{
    #region Variables

    private AssetServiceOptions _options = new();
    private readonly ConcurrentDictionary<EntityAssetIdentifier, EntityLookupEntry> _entryLookup = [];
    private readonly ConcurrentDictionary<ModuleAssetIdentifier, IModuleLoader> _loaderLookup = [];

    #endregion

    #region IAssetService

    public IAssetDatabaseContext DatabaseContext => database;

    public Task<Output> InitializeAsync(AssetServiceOptions? options = null, CancellationToken cancellationToken = default)
    {
        _options = options ?? new();
        database.StartInitialization();
        return assetManager.InitializeDatabaseAsync(database, cancellationToken);
    }

    public void Update(TimeSpan deltaTime)
    {
        foreach (var key in _loaderLookup.Keys.ToArray())
        {
            var loader = _loaderLookup[key];
            loader.Update(deltaTime);
            if (loader.LoadProgress.IsComplete)
            {
                _loaderLookup.Remove(key, out _);
            }
        }

        if (_options.InstantiatorIdleDisposalTimeout is null || _options.InstantiatorIdleDisposalTimeout <= TimeSpan.Zero)
        {
            return;
        }

        var now = DateTime.Now;
        foreach (var key in _entryLookup.Keys.Where(key => (now - _entryLookup[key].LastUsed) >= _options.InstantiatorIdleDisposalTimeout).ToArray())
        {
            _entryLookup[key].Instantiator.Dispose();
            _entryLookup.Remove(key, out _);
        }
    }

    public async ValueTask<Output<TEntity>> InstantiateAsync<TEntity, TTransform>(InstantiationParameters<TTransform> parameters, CancellationToken cancellationToken = default)
        where TEntity: class
        where TTransform : ITransform
    {
        var descriptor = parameters.AssetReference.AssetIdentifier is not null
            ? database.GetEntity(parameters.AssetReference.AssetIdentifier.Value)
            : null;

        var assetId = parameters.AssetReference.AssetIdentifier;
        IInvoker? descriptorInvoker = null;
        IAssetInstantiator<TEntity, TTransform> instantiator;
        EntityLookupEntry? entry = null;

        if (assetId is not null && _entryLookup.TryGetValue(assetId.Value, out entry))
        {
            if (_options.InstantiatorIdleDisposalTimeout.HasValue)
            {
                entry.LastUsed = DateTime.Now;
            }
            instantiator = (IAssetInstantiator<TEntity, TTransform>)entry.Instantiator;
            descriptorInvoker = entry.DescriptorInvoker;
        }
        else
        {
            var getInstantiator = await assetManager.GetInstantiatorAsync<TEntity, TTransform>(parameters.AssetReference, cancellationToken);
            if (!getInstantiator.IsSuccessful)
            {
                return getInstantiator.As<TEntity>();
            }

            instantiator = getInstantiator.Data;
        }

        var entity = instantiator.Instantiate(parameters.Transform, entity =>
        {
            if (assetId is not null && entry is null)
            {
                descriptorInvoker = descriptor is not null
                    ? TryCreateDescriptorInvoker(entity.GetType(), descriptor.GetType())
                    : null;

                entry = new(instantiator, descriptorInvoker);
                _entryLookup[assetId.Value] = entry;
            }

            if (descriptor is not null)
            {
                descriptorInvoker?.FastInvoke(entity, [descriptor]);
            }
        });

        if (parameters.Services is not null)
        {
            DependencyInjector.InjectDependencies(parameters.Services, entity);
        }

        return Out.Success(entity);
    }

    public IModuleLoadContext? LoadModule<TLoadParameters>(TLoadParameters parameters)
        where TLoadParameters : ModuleLoadParameters
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        var moduleDescriptor = database.GetModule(parameters.ModuleIdentifier);
        if (moduleDescriptor is null)
        {
            return null;
        }
        if (_loaderLookup.TryGetValue(moduleDescriptor.AssetIdentifier, out var loader))
        {
            return loader;
        }

        var loaderType = moduleDescriptor.GetModuleLoaderType();
        if (!typeof(IModuleLoader).IsAssignableFrom(loaderType))
        {
            return null;
        }

        loader = (IModuleLoader)ActivatorUtilities.CreateInstance(services, loaderType, moduleDescriptor, parameters);
        _loaderLookup[moduleDescriptor.AssetIdentifier] = loader;

        if (parameters.LoadBehavior is ModuleLoadBehavior.Replace)
        {
            loadScreen.Initialize(parameters, loader);
        }

        return loader;
    }

    public IModuleDescriptor? GetModuleDescriptor(ModuleAssetIdentifier identifier)
    => database.GetModule(identifier);

    public IEntityDescriptor? GetEntityDescriptor(EntityAssetIdentifier identifier)
        => database.GetEntity(identifier);

    public IEnumerable<IModuleDescriptor> GetModuleDescriptors(AssetSearchOptions? searchOptions = null)
        => database.GetModules(searchOptions);

    public IEnumerable<IEntityDescriptor> GetEntityDescriptors(AssetSearchOptions? searchOptions = null)
        => database.GetEntities(searchOptions);

    #endregion

    #region Helpers

    private IInvoker? TryCreateDescriptorInvoker(Type entityType, Type descriptorType)
    {
        var entityInterface = entityType.GetInterfaces()
                                        .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGameEntity<>));

        if (entityInterface is null)
        {
            return null;
        }

        var entityDescriptorProperty = entityInterface.GetProperty(nameof(IGameEntity<>.Descriptor));

        return entityDescriptorProperty.PropertyType.IsAssignableFrom(descriptorType)
            ? InvokerFactory.CreateInvoker(entityType, entityDescriptorProperty)
            : null;
    }

    #endregion
}
