namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents a strongly typed asset tag name
/// </summary>
/// <param name="Name">The name of the tag</param>
public readonly record struct AssetTagName(string Name)
{
    #region Operators

    public static implicit operator string(AssetTagName name)
        => name.Name;

    public static implicit operator AssetTagName(string name)
        => new(name);

    #endregion
}
