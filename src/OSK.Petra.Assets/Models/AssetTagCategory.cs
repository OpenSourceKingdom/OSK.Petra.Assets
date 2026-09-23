namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents a strongly typed category name
/// </summary>
/// <param name="Name">The name of the categroy</param>
public readonly record struct AssetTagCategory(string Name)
{
    #region Operators

    public static implicit operator string(AssetTagCategory assetTagCategory)
        => assetTagCategory.Name;

    public static implicit operator AssetTagCategory(string name)
        => new(name);

    #endregion
}
