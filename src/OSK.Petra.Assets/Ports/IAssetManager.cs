using OSK.Operations.Outputs.Models;
using System.Threading;
using System.Threading.Tasks;
using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Ports;

public interface IAssetManager
{
    Task<Output> InitializeDatabaseAsync(IAssetInitializationContext context, CancellationToken cancellationToken = default);

    Task<Output<IAssetInstantiator<TEntity, TTransform>>> GetInstantiatorAsync<TEntity, TTransform>(IEntityAssetReference<TTransform> assetReference,
        CancellationToken cancellationToken = default)
        where TEntity: class
        where TTransform: ITransform;
}
