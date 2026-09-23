namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes an asset that is expected to be instantiated within a module
/// </summary>
public interface IEntityDescriptor: IAssetDescriptor
{
    /// <summary>
    /// The unique identifier for the entity
    /// </summary>
    EntityAssetIdentifier AssetIdentifier { get; }
}
