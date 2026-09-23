using System.Collections.Generic;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Provides details and insights into a particular asset without needing to instantiate or load it
/// </summary>
public interface IAssetDescriptor
{
    /// <summary>
    /// The name of the asset
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of the asset
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The path to the icon that represents the asset
    /// </summary>
    string IconPath { get; }

    /// <summary>
    /// The data path to the asset
    /// </summary>
    string AssetPath { get; }

    /// <summary>
    /// The size of the data asset that is stored
    /// </summary>
    long? Size { get; }

    /// <summary>
    /// A collection of asset tags associated with the asset that can be used with search APIs
    /// </summary>
    IEnumerable<AssetTag> Tags { get; }
}
