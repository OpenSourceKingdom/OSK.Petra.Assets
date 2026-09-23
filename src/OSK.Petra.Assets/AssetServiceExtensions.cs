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

        /// <summary>
        /// Instantiates an object immediately, given a template
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to instantiate</typeparam>
        /// <typeparam name="TTransform">The type of transform the entity utilizes</typeparam>
        /// <param name="reference">The reference to the template</param>
        /// <param name="transform">The transform to apply to the entity</param>
        /// <returns>The entity</returns>
        /// <exception cref="InvalidOperationException">If an error occurs when getting the entity</exception>
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

        /// <summary>
        /// Instantiates an object immediately, given a template
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to instantiate</typeparam>
        /// <typeparam name="TTransform">The type of transform the entity utilizes</typeparam>
        /// <param name="reference">The reference to the template</param>
        /// <param name="transform">The transform to apply to the entity</param>
        /// <param name="services">The service provider to configure and setup the new entity with</param>
        /// <returns>The entity</returns>
        /// <exception cref="InvalidOperationException">If an error occurs when getting the entity</exception>
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

        /// <summary>
        /// Attempts to instantiate an entity immediately, provided a descriptor
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to instantiate</typeparam>
        /// <typeparam name="TTransform">The type of transform the entity utilizes</typeparam>
        /// <param name="transform">The transform to apply to the entity</param>
        /// <returns>The entity</returns>
        /// <exception cref="InvalidOperationException">If an error occurs when getting the entity</exception>
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

        /// <summary>
        /// Attempts to instantiate an entity immediately, provided a descriptor
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to instantiate</typeparam>
        /// <typeparam name="TTransform">The type of transform the entity utilizes</typeparam>
        /// <param name="transform">The transform to apply to the entity</param>
        /// <param name="services">The service provider to configure and setup the new entity with</param>
        /// <returns>The entity</returns>
        /// <exception cref="InvalidOperationException">If an error occurs when getting the entity</exception>
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

        /// <summary>
        /// Attempts to instantiate an entity asynchronously using a provided descriptor
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to instantiate</typeparam>
        /// <typeparam name="TTransform">The type of transform the entity utilizes</typeparam>
        /// <param name="transform">The transform to apply to the entity</param>
        /// <param name="services">The service provider to configure and setup the new entity with</param>
        /// <returns>The entity</returns>
        public ValueTask<Output<TEntity>> InstantiateAsync<TEntity, TTransform>(IEntityDescriptor<TEntity, TTransform> descriptor, TTransform transform, CancellationToken cancellationToken = default)
            where TEntity : class
            where TTransform : ITransform
            => service.InstantiateAsync<TEntity, TTransform>(new InstantiationParameters<TTransform>()
            {
                Transform = transform,
                AssetReference = descriptor.GetAssetReference()
            }, cancellationToken);

        /// <summary>
        /// Attempts to instantiate an entity asynchronously using a provided descriptor
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to instantiate</typeparam>
        /// <typeparam name="TTransform">The type of transform the entity utilizes</typeparam>
        /// <param name="transform">The transform to apply to the entity</param>
        /// <param name="services">The service provider to configure and setup the new entity with</param>
        /// <param name="services">The service provider to configure and setup the new entity with</param>
        /// <returns>The entity</returns>
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
