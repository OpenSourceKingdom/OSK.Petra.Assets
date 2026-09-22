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
    /// The collection of tag matchers to use when matching assets for a search
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Empty/null filters will allow any tag</item>
    /// </list>
    /// </remarks>
    public AssetTagMatcher[]? TagMatchers { get; init; }

    /// <summary>
    /// The behavior used when validating tags against the collection of tag matchers
    /// </summary>
    public TagClauseMatchBehavior TagClauseMatchBehavior { get; init; }

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
            TagMatchers = TagMatchers,
            TagClauseMatchBehavior = TagClauseMatchBehavior
        };

    /// <summary>
    /// Creates a search options with the included tag matchers
    /// </summary>
    /// <param name="matcher">The matcher to include in the search</param>
    /// <returns>A search options with the included data</returns>
    public AssetSearchOptions WithTagMatcher(AssetTagMatcher matcher)
        => new()
        {
            AssetPackageIds = AssetPackageIds,
            TagMatchers = TagMatchers is null
                ? [matcher]
                : [.. TagMatchers.Append(matcher)],
            TagClauseMatchBehavior = TagClauseMatchBehavior
        };

    /// <summary>
    /// Creates a search options with the included tag match behavior
    /// </summary>
    /// <param name="matchBehavior">The behavior to use when matching the tags</param>
    /// <returns>A search options with the included data</returns>
    public AssetSearchOptions WithTagMatcherClauseBehavior(TagClauseMatchBehavior matchBehavior)
        => new()
        {
            AssetPackageIds = AssetPackageIds,
            TagMatchers = TagMatchers,
            TagClauseMatchBehavior = matchBehavior
        };

    #endregion
}
