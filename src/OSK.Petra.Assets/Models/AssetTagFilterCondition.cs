using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Defines how the values within an <see cref="AssetTagFilter"/> are matched.
/// </summary>
public enum AssetTagFilterCondition
{
    /// <summary>
    /// The tag matches when it has at least one of the specified tag values.
    /// </summary>
    Any,

    /// <summary>
    /// The tag matches only when it has all of the specified tag values.
    /// </summary>
    All
}
