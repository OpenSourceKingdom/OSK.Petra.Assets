namespace OSK.Petra.Assets.Models;

public interface IEntityDescriptor: IAssetDescriptor
{
    EntityAssetIdentifier AssetIdentifier { get; }
}
