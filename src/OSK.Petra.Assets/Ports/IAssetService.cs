using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Assets.Ports;

/// <summary>
/// The main API and entry point into the asset system
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided)]
public interface IAssetService
{
    /// <summary>
    /// The database context that provides details into the current state of the asset system
    /// </summary>
    IAssetDatabaseContext DatabaseContext { get; }

    /// <summary>
    /// Starts the asset initialization process. This can be used multiple times to reload the database with the latest assets available.
    /// </summary>
    /// <param name="options">The service options to use with the asset system</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output describing the success of the operation</returns>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Initializing a database will likely result in any related cached information being cleaned up</item>
    /// </list>
    /// </remarks>
    Task<Output> InitializeAsync(AssetServiceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the internal state of the asset service based on the provided time since the last frame
    /// </summary>
    /// <param name="deltaTime">The amount of time since last frame</param>
    void Update(TimeSpan deltaTime);

    /// <summary>
    /// Creates the specified entity with the provided parameters
    /// </summary>
    /// <typeparam name="TEntity">The entity type being instantiated</typeparam>
    /// <typeparam name="TTransform">The transform type the entity utilizes</typeparam>
    /// <param name="parameters">The instantiation parmaeters to create and configure the entity</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output describing the result of the oepration</returns>
    ValueTask<Output<TEntity>> InstantiateAsync<TEntity, TTransform>(InstantiationParameters<TTransform> parameters, CancellationToken cancellationToken = default)
        where TEntity: class
        where TTransform: ITransform;

    /// <summary>
    /// Starts the loading process for a module given the load parameters
    /// </summary>
    /// <typeparam name="TLoadParameters">The type of load parameters the module needs to load</typeparam>
    /// <param name="parameters">The load parameters to load the module</param>
    /// <returns>A module context that details the load progress and related information, if the module exists</returns>
    IModuleLoadContext? LoadModule<TLoadParameters>(TLoadParameters parameters)
        where TLoadParameters: ModuleLoadParameters;

    /// <summary>
    /// Gets the module descriptor using the provided identifier
    /// </summary>
    /// <param name="identifier">The unique identifier for the asset</param>
    /// <returns>The descriptor, if found</returns>
    IModuleDescriptor? GetModuleDescriptor(ModuleAssetIdentifier identifier);

    /// <summary>
    /// Gets the entity descriptor using the provided identifier
    /// </summary>
    /// <param name="identifier">The unique identifier for the asset</param>
    /// <returns>The descriptor, if found</returns>
    IEntityDescriptor? GetEntityDescriptor(EntityAssetIdentifier identifier);

    /// <summary>
    /// Get the list of descriptors, provided a set of search options
    /// </summary>
    /// <param name="searchOptions">The search options to apply to the search</param>
    /// <returns>The collection of module descriptors</returns>
    IEnumerable<IModuleDescriptor> GetModuleDescriptors(AssetSearchOptions? searchOptions = null);

    /// <summary>
    /// Get the list of descriptors, provided a set of search options
    /// </summary>
    /// <param name="searchOptions">The search options to apply to the search</param>
    /// <returns>The collection of entity descriptors</returns>
    IEnumerable<IEntityDescriptor> GetEntityDescriptors(AssetSearchOptions? searchOptions = null);
}
