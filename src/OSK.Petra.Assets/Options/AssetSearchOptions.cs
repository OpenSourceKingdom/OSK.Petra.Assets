using OSK.Petra.Assets.Models;
using System;
using System.Linq;

namespace OSK.Petra.Assets.Options;

public readonly struct AssetSearchOptions
{
    #region Variables

    public Guid[]? AssetPackageIds { get; init; }

    public AssetTagMatcher[]? TagMatchers { get; init; }

    public TagClauseMatchBehavior TagClauseMatchBehavior { get; init; }

    #endregion

    #region Api

    public AssetSearchOptions WithPackageIds(params Guid[] assetPackageIds)
        => new()
        {
            AssetPackageIds = AssetPackageIds is null
                ? [.. assetPackageIds]
                : [.. AssetPackageIds.Concat(assetPackageIds)],
            TagMatchers = TagMatchers,
            TagClauseMatchBehavior = TagClauseMatchBehavior
        };

    public AssetSearchOptions WithTagMatcher(AssetTagMatcher matcher)
        => new()
        {
            AssetPackageIds = AssetPackageIds,
            TagMatchers = TagMatchers is null
                ? [matcher]
                : [.. TagMatchers.Append(matcher)],
            TagClauseMatchBehavior = TagClauseMatchBehavior
        };

    public AssetSearchOptions WithTagMatcherClauseBehavior(TagClauseMatchBehavior matchBehavior)
        => new()
        {
            AssetPackageIds = AssetPackageIds,
            TagMatchers = TagMatchers,
            TagClauseMatchBehavior = matchBehavior
        };

    #endregion
}
