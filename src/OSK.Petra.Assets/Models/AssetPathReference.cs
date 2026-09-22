using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// An asset reference that is based on a path to th ebject. i.e. the asset will need to be loaded into memory to be usable.
/// </summary>
/// <typeparam name="TTransform">The transform the asset utilizes</typeparam>
/// <param name="path">The path to the asset</param>
/// <param name="assetIdentifier">An optional identifier that is used to provide caching and descriptor handling</param>
public readonly struct AssetPathReference<TTransform>(string path, EntityAssetIdentifier? assetIdentifier = null): IEntityAssetReference<TTransform>
   where TTransform: ITransform 
{
    #region Variables

    /// <summary>
    /// The path to the asset
    /// </summary>
    public string Path { get; } = string.IsNullOrWhiteSpace(path) ? throw new ArgumentNullException(nameof(path), "An asset path reference can not have an empty path") : path;

    /// <summary>
    /// The identifier that is associated with the asset that is used for caching, descriptor, and similar handling
    /// </summary>
    public EntityAssetIdentifier? AssetIdentifier { get; } = assetIdentifier;

    #endregion
}
