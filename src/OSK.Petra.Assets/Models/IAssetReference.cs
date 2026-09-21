namespace OSK.Petra.Assets.Models;

public interface IEntityAssetReference<TTransform>
    where TTransform : ITransform
{
    EntityAssetIdentifier? AssetIdentifier { get; }
}
