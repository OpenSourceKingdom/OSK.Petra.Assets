using System.Collections.Generic;

namespace OSK.Petra.Assets.Models;

public interface IAssetDescriptor
{
    string Name { get; }

    string? Description { get; }

    string IconPath { get; }

    string AssetPath { get; }

    long? Size { get; }

    IEnumerable<AssetTag> Tags { get; }
}
