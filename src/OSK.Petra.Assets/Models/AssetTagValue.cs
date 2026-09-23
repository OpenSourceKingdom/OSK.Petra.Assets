namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents a strongly typed asset tag value
/// </summary>
/// <param value="Value">The value of the tag</param>
public readonly record struct AssetTagValue(string Value)
{
    #region Operators

    public static implicit operator string(AssetTagValue value)
        => value.Value;

    public static implicit operator AssetTagValue(string value)
        => new(value);

    #endregion
}
