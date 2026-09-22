namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents a combination of an asset category and asset tag name
/// </summary>
/// <param name="Category">The category the tag falls into</param>
/// <param name="Name">The name/value of the tag</param>
public readonly record struct AssetTag(AssetTagCategory Category, AssetTagName Name)
{
}
