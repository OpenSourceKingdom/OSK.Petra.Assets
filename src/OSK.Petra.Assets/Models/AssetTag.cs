using System.Collections.Generic;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Represents a combination of an asset category and a collection of asset tag values
/// </summary>
/// <param name="Category">The category the tag falls into</param>
/// <param name="Values">The collection of tag values</param>
public readonly struct AssetTag(AssetTagCategory category, IEnumerable<AssetTagValue> values)
{
    #region Constructors

    public AssetTag(AssetTagCategory category, params AssetTagValue[] values)
        : this(category, (IEnumerable<AssetTagValue>) values)
    {
    }

    #endregion

    #region Variables

    /// <summary>
    /// The category the tag is in
    /// </summary>
    public readonly AssetTagCategory Category => category;

    /// <summary>
    /// The collection of tag values that this tag possesses
    /// </summary>
    public readonly IReadOnlyCollection<AssetTagValue> Values { get; } = [.. values];

    #endregion
}
