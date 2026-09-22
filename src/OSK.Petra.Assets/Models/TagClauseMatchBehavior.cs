namespace OSK.Petra.Assets.Models;

/// <summary>
/// Describes how a set of <see cref="AssetTagMatcher"/> should be used when matching asset tags
/// </summary>
public enum TagClauseMatchBehavior
{
    /// <summary>
    /// Asset tag matching will be based on an OR style clause across a collection of tag matchers
    /// </summary>
    Any,

    /// <summary>
    /// Asset tag matching will be based on an AND style clause across a collection of tag matchers 
    /// </summary>
    All
}
