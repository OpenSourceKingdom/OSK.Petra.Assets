using System;
using System.Linq;

namespace OSK.Petra.Assets.Models;

public readonly struct AssetTagMatcher(AssetTagCategory category, AssetTagName[] tagFilter)
{
    #region Helpers

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
