using OSK.Petra.Assets.Models;
using System.Linq;

namespace OSK.Petra.Assets.Options;

public static class AssetSearchOptionsExtensions
{
    extension(AssetSearchOptions options)
    {
        /// <summary>
        /// Creates a search options with a tag matcher given string values for the category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="tagFilter">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagMatcher(string category, params string[] tagFilter)
            => options.WithTagMatcher(new AssetTagMatcher(category, [.. tagFilter.Select(tag => new AssetTagName(tag))]));

        /// <summary>
        /// Creates a search options with a tag matcher given the asset category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="tagFilter">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagMatcher(AssetTagCategory category, params AssetTagName[] tagFilter)
            => options.WithTagMatcher(new AssetTagMatcher(category, tagFilter));
    }
}
