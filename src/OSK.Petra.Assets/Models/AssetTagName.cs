namespace OSK.Petra.Assets.Models;

public readonly record struct AssetTagName(string Name)
{
    #region Operators

    public static implicit operator string(AssetTagName name)
        => name.Name;

    public static implicit operator AssetTagName(string name)
        => new(name);

    #endregion
}
