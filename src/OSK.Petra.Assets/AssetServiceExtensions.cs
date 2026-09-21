using OSK.Operations.Outputs.Models;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Ports;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Assets;

public static class AssetServiceExtensions
{
    extension(IAssetService service)
    {
        #region Instantiate

        public TEntity Instantiate<TEntity, TTransform>(AssetTemplateReference<TEntity, TTransform> reference, TTransform transform)
            where TEntity : class
            where TTransform : ITransform
        {
            var entityOutput = service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = reference
            }).Result;

            if (entityOutput?.Data is null)
            {
                throw new InvalidOperationException($"An entity that was expected to exist did not. Entity Type: {typeof(TEntity).FullName}");
            }

            return entityOutput.Data;
        }

        public TEntity Instantiate<TEntity, TTransform>(AssetTemplateReference<TEntity, TTransform> reference, TTransform transform, IServiceProvider services)
            where TEntity : class
            where TTransform : ITransform
        {
            var entityOutput = service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = reference,
                Services = services
            }).Result;

            if (entityOutput?.Data is null)
            {
                throw new InvalidOperationException($"An entity that was expected to exist did not. Entity Type: {typeof(TEntity).FullName}");
            }

            return entityOutput.Data;
        }

        #endregion

        #region IEntityDescriptor<Entity, Transform>

        public TEntity Instantiate<TEntity, TTransform>(IEntityDescriptor<TEntity, TTransform> descriptor, TTransform transform)
            where TEntity : class
            where TTransform : ITransform
        {
            var entityOutput = service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = descriptor.GetAssetReference()
            }).Result;

            if (entityOutput?.Data is null)
            {
                throw new InvalidOperationException($"An entity that was expected to exist did not. Entity Type: {typeof(TEntity).FullName}");
            }

            return entityOutput.Data;
        }

        public TEntity Instantiate<TEntity, TTransform>(IEntityDescriptor<TEntity, TTransform> descriptor, TTransform transform, IServiceProvider services)
            where TEntity : class
            where TTransform : ITransform
        {
            var entityOutput = service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = descriptor.GetAssetReference(),
                Services = services
            }).Result;

            if (entityOutput?.Data is null)
            {
                throw new InvalidOperationException($"An entity that was expected to exist did not. Entity Type: {typeof(TEntity).FullName}");
            }

            return entityOutput.Data;
        }

        public ValueTask<Output<TEntity>> InstantiateAsync<TEntity, TTransform>(IEntityDescriptor<TEntity, TTransform> descriptor, TTransform transform, CancellationToken cancellationToken = default)
            where TEntity : class
            where TTransform : ITransform
            => service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = descriptor.GetAssetReference()
            }, cancellationToken);

        public ValueTask<Output<TEntity>> InstantiateAsync<TEntity, TTransform>(IEntityDescriptor<TEntity, TTransform> descriptor, TTransform transform, IServiceProvider services, CancellationToken cancellationToken = default)
            where TEntity : class
            where TTransform : ITransform
            => service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = descriptor.GetAssetReference(),
                Services = services
            }, cancellationToken);

        #endregion
    }
}
