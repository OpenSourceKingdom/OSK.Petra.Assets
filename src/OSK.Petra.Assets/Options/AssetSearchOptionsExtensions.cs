using OSK.Petra.Assets.Models;
using System;
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
        /// <param name="tagValues">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagFilter(string category, params string[] tagValues)
            => options.WithTagFilter(new AssetTagFilter(category, [.. tagValues.Select(tag => new AssetTagValue(tag))]));

        /// <summary>
        /// Creates a search options with a tag matcher given the asset category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="tagValues">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagFilter(AssetTagCategory category, params AssetTagValue[] tagValues)
            => options.WithTagFilter(new AssetTagFilter(category, tagValues));

        /// <summary>
        /// Creates a search options with a tag matcher given string values for the category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="tagComparison">Describes the style of comparison that should be performed for tag validation</param>
        /// <param name="tagValues">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagFilter(string category, StringComparison tagComparison, params string[] tagValues)
            => options.WithTagFilter(new AssetTagFilter(category, [.. tagValues.Select(tag => new AssetTagValue(tag))], tagComparison));

        /// <summary>
        /// Creates a search options with a tag matcher given the asset category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="tagComparison">Describes the style of comparison that should be performed for tag validation</param>
        /// <param name="tagValues">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagFilter(AssetTagCategory category, StringComparison tagComparison, params AssetTagValue[] tagValues)
            => options.WithTagFilter(new AssetTagFilter(category, tagValues, tagComparison));

        /// <summary>
        /// Creates a search options with a tag matcher given string values for the category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="filterCondition">Describes the style of condition a tag must match to satisfy this filter</param>
        /// <param name="tagValues">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagFilter(string category, AssetTagFilterCondition filterCondition, params string[] tagValues)
            => options.WithTagFilter(new AssetTagFilter(category, [.. tagValues.Select(tag => new AssetTagValue(tag))], filterCondition));

        /// <summary>
        /// Creates a search options with a tag matcher given the asset category and tag filter
        /// </summary>
        /// <param name="category">The category to match</param>
        /// <param name="filterCondition">Describes the style of condition a tag must match to satisfy this filter</param>
        /// <param name="tagValues">A collection of valid tags</param>
        /// <returns>A search options with the included data</returns>
        public AssetSearchOptions WithTagFilter(AssetTagCategory category, AssetTagFilterCondition filterCondition, params AssetTagValue[] tagValues)
            => options.WithTagFilter(new AssetTagFilter(category, tagValues, filterCondition));
    }
}
