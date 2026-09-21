using System;

namespace OSK.Petra.Assets.Models;

public readonly struct AssetPathReference<TTransform>(string path, EntityAssetIdentifier? assetIdentifier = null): IEntityAssetReference<TTransform>
   where TTransform: ITransform 
{
    #region Variables

    public string Path { get; } = string.IsNullOrWhiteSpace(path) ? throw new ArgumentNullException(nameof(path), "An asset path reference can not have an empty path") : path;

    public EntityAssetIdentifier? AssetIdentifier { get; } = assetIdentifier;

    #endregion
}
