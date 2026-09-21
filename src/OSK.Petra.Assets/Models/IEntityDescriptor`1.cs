namespace OSK.Petra.Assets.Models;

public interface IEntityDescriptor<TEntity, TTransform>: IEntityDescriptor
    where TEntity : class
    where TTransform : ITransform
{
    IEntityAssetReference<TTransform> GetAssetReference();
}
