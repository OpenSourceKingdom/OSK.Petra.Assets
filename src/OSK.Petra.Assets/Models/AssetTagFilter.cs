using System;
using System.Linq;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Provides tag filtering
/// </summary>
/// <param name="category">The category that will be matched against</param>
/// <param name="tagValues">The collection of tagValues that satisfy the filter. Empty/null allows all tagValues that match the category</param>
/// <param name="filterCondition">Describes the style of condition a tag must match to satisfy this filter</param>
/// <param name="tagComparison">Describes the style of comparison that should be performed for tag validation</param>
public readonly struct AssetTagFilter(AssetTagCategory category, AssetTagValue[] tagValues, AssetTagFilterCondition filterCondition, StringComparison tagComparison)
{
    #region Constructors

    /// <summary>
    /// Provides tag filtering, using default values for the filter condition and tag comparison.
    /// </summary>
    /// <param name="category">The category that will be matched against</param>
    /// <param name="tags">The collection of tagValues that satisfy the filter. Empty/null allows all tagValues that match the category</param>
    public AssetTagFilter(AssetTagCategory category, AssetTagValue[] tags)
        : this(category, tags, AssetTagFilterCondition.Any, StringComparison.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Provides tag filtering, using default values for the tag comparison
    /// </summary>
    /// <param name="category">The category that will be matched against</param>
    /// <param name="tags">The collection of tagValues that satisfy the filter. Empty/null allows all tagValues that match the category</param>
    /// <param name="filterCondition">Describes the style of condition a tag must match to satisfy this filter</param>
    public AssetTagFilter(AssetTagCategory category, AssetTagValue[] tags, AssetTagFilterCondition filterCondition)
        : this(category, tags, filterCondition, StringComparison.OrdinalIgnoreCase) 
    { 
    }

    /// <summary>
    /// Provides tag filtering, using default values for the filter condition
    /// </summary>
    /// <param name="category">The category that will be matched against</param>
    /// <param name="tags">The collection of tagValues that satisfy the filter. Empty/null allows all tagValues that match the category</param>
    /// <param name="tagComparison">Describes the style of comparison that should be performed for tag validation</param>
    public AssetTagFilter(AssetTagCategory category, AssetTagValue[] tags, StringComparison tagComparison)
        : this(category, tags, AssetTagFilterCondition.All, tagComparison)
    {
    }

    #endregion

    #region Api

    /// <summary>
    /// Determins if the tag matcher matches with the provider tag
    /// </summary>
    /// <param name="tag">The asset tag to compare against</param>
    /// <returns>Whether the tag matched with this matcher</returns>
    public bool Matches(AssetTag tag)
    {
        if (!string.IsNullOrWhiteSpace(category) && !tag.Category.Name.Equals(category, tagComparison))
        {
            return false;
        }

        if (tagValues is { Length :> 0})
        {
            if (tag.Values is null || tag.Values.Count is 0)
            {
                return false;
            }

            var comparer = StringComparer.FromComparison(tagComparison);
            var tagValidation = filterCondition is AssetTagFilterCondition.Any
                ? tagValues.Any(tagValue => tag.Values.Any(v => comparer.Equals(v.Value, tagValue.Value)))
                : tagValues.All(tagValue => tag.Values.Any(v => comparer.Equals(v.Value, tagValue.Value)));

            if (!tagValidation)
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}
