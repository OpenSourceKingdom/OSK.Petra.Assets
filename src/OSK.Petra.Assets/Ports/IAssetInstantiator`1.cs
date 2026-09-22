using OSK.Hexagonal.MetaData;
using OSK.Petra.Assets.Models;
using System;

namespace OSK.Petra.Assets.Ports;

/// <summary>
/// Represents an instantiator for an entity that is strongly typed
/// </summary>
/// <typeparam name="TEntity">The type of entity the instantiator creates</typeparam>
/// <typeparam name="TTransform">The type of transform the entity uses</typeparam>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface IAssetInstantiator<TEntity, TTransform>: IAssetInstantiator
    where TEntity: class
    where TTransform: ITransform
{
    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="transform">The transform to apply to the entity</param>
    /// <param name="configurator">A custom configurator to apply to an entity after it has been created</param>
    /// <returns>The entity</returns>
    TEntity Instantiate(TTransform transform, Action<TEntity>? configurator = null);
}
