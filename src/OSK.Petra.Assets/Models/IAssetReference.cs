namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents a particular reference to an asset and determines how the asset will be loaded and instantiated in a module
/// </summary>
/// <typeparam name="TTransform">The tpye of transform that the asset utilizes</typeparam>
public interface IEntityAssetReference<TTransform>
    where TTransform : ITransform
{
    /// <summary>
    /// An asset identifier that can be used with the asset system to determine caching, descriptor, and similar process handling
    /// </summary>
    EntityAssetIdentifier? AssetIdentifier { get; }
}
