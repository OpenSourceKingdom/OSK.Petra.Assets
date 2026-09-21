namespace OSK.Petra.Assets.Models;

public readonly record struct AssetTagCategory(string Name)
{
    #region Operators

    public static implicit operator string(AssetTagCategory assetTagCategory)
        => assetTagCategory.Name;

    public static implicit operator AssetTagCategory(string name)
        => new(name);

    #endregion
}
