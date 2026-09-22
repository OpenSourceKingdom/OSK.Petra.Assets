using OSK.Hexagonal.MetaData;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Assets.Ports;

/// <summary>
/// Provides management of the assets and integrates with the asset system to give access to entity instantiators and loading the database
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface IAssetManager
{
    /// <summary>
    /// Initializes the asset database by discovering and adding required asset descriptors
    /// </summary>
    /// <param name="context">The asset initialization context for the current process</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output indicating the result of the operation</returns>
    Task<Output> InitializeDatabaseAsync(IAssetInitializationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an instantiator capable of creating the entities of the given tyype
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the instantiator creates</typeparam>
    /// <typeparam name="TTransform">The transform type the entity utilizes</typeparam>
    /// <param name="assetReference">The specific reference to use with the instantiator</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>An output task to get the instantiator</returns>
    Task<Output<IAssetInstantiator<TEntity, TTransform>>> GetInstantiatorAsync<TEntity, TTransform>(IEntityAssetReference<TTransform> assetReference, CancellationToken cancellationToken = default)
        where TEntity: class
        where TTransform: ITransform;
}
