using System;
using System.Linq;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Provides tag matching capability
/// </summary>
/// <param name="category">The category that will be matched against</param>
/// <param name="tagFilter">The filter of tags that are allowed. Empty/null allows all tags that match the category</param>
public readonly struct AssetTagMatcher(AssetTagCategory category, AssetTagName[] tagFilter)
{
    #region Helpers

    /// <summary>
    /// Determins if the tag matcher matches with the provider tag
    /// </summary>
    /// <param name="tag">The asset tag to compare against</param>
    /// <returns>Whether the tag matched with this matcher</returns>
    public bool Matches(AssetTag tag)
    {
        if (!string.IsNullOrWhiteSpace(category) && !tag.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        if (tagFilter is { Length :> 0} && (string.IsNullOrWhiteSpace(tag.Name) || !tagFilter.Select(tag => tag.Name).Contains(tag.Name.Name, StringComparer.OrdinalIgnoreCase)))
        {
            return false;
        }

        return true;
    }

    #endregion
}
