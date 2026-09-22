namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes an entity asset that has a strongly typed expected transform 
/// </summary>
/// <typeparam name="TEntity">The entity type the descriptor refers to</typeparam>
/// <typeparam name="TTransform">The transform type the entity utilizes</typeparam>
public interface IEntityDescriptor<TEntity, TTransform>: IEntityDescriptor
    where TEntity : class
    where TTransform : ITransform
{
    /// <summary>
    /// Get the asset reference required to fully load and instantiate the entity within the asset system
    /// </summary>
    /// <returns>The reference needed to instantiate the entity</returns>
    IEntityAssetReference<TTransform> GetAssetReference();
}
