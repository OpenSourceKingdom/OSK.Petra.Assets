using OSK.Petra.Assets.Models;
using System.Linq;

namespace OSK.Petra.Assets.Options;

public static class AssetSearchOptionsExtensions
{
    extension(AssetSearchOptions options)
    {
        public AssetSearchOptions WithTagMatcher(string category, params string[] tagFilter)
            => options.WithTagMatcher(new AssetTagMatcher(category, [.. tagFilter.Select(tag => new AssetTagName(tag))]));

        public AssetSearchOptions WithTagMatcher(AssetTagCategory category, params AssetTagName[] tagFilter)
            => options.WithTagMatcher(new AssetTagMatcher(category, tagFilter));
    }
}
