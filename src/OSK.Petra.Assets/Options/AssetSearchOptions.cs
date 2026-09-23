using OSK.Petra.Assets.Models;
using System;
using System.Linq;

namespace OSK.Petra.Assets.Options;

/// <summary>
/// A set of options that can be used to search for assets
/// </summary>
public readonly struct AssetSearchOptions
{
    #region Variables

    /// <summary>
    /// The collection of packages to specifically search for. 
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Empty/null filters will allow any packages</item>
    /// </list>
    /// </remarks>
    public Guid[]? AssetPackageIds { get; init; }

    /// <summary>
    /// The collection of tag filters to use when matching assets for a search
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Empty/null filters will allow any tag</item>
    /// </list>
    /// </remarks>
    public AssetTagFilter[]? TagFilters { get; init; }

    #endregion

    #region Api

    /// <summary>
    /// Creates a search options with the included package ids
    /// </summary>
    /// <param name="assetPackageIds">The package ids to include in the search</param>
    /// <returns>A search options with the included data</returns>
    public AssetSearchOptions WithPackageIds(params Guid[] assetPackageIds)
        => new()
        {
            AssetPackageIds = AssetPackageIds is null
                ? [.. assetPackageIds]
                : [.. AssetPackageIds.Concat(assetPackageIds)],
            TagFilters = TagFilters
        };

    /// <summary>
    /// Creates a search options with the included tag matchers
    /// </summary>
    /// <param name="filter">The filter to include in the search</param>
    /// <returns>A search options with the included data</returns>
    public AssetSearchOptions WithTagFilter(AssetTagFilter filter)
        => new()
        {
            AssetPackageIds = AssetPackageIds,
            TagFilters = TagFilters is null
                ? [filter]
                : [.. TagFilters.Append(filter)]
        };

    #endregion
}
