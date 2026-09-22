using System;

namespace OSK.Petra.Assets;

/// <summary>
/// A collection of well known data information
/// </summary>
public static class AssetIdentifiers
{
    /// <summary>
    /// The package id used if no package information is specified with a given asset
    /// </summary>
    public static Guid DefaultPackageId = Guid.Empty;
}
