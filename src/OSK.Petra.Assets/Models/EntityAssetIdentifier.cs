using System;

namespace OSK.Petra.Assets.Models;

public readonly struct EntityAssetIdentifier : IEquatable<EntityAssetIdentifier>
{
    #region Variables

    public Guid AssetId { get; init; }

    public Guid AssetPackageId { get; init; }

    #endregion

    #region Constructors

    public EntityAssetIdentifier(Guid id)
        : this(AssetIdentifiers.DefaultPackageId, id)
    {
    }

    public EntityAssetIdentifier(Guid packageId, Guid id)
    {
        AssetPackageId = packageId;
        AssetId = id;
    }

    #endregion

    #region Equality

    public bool Equals(EntityAssetIdentifier other)
        => other.AssetPackageId == AssetPackageId && other.AssetId.Equals(AssetId);

    public override bool Equals(object? obj)
        => obj is EntityAssetIdentifier other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(AssetPackageId, AssetId);

    public static bool operator ==(EntityAssetIdentifier left, EntityAssetIdentifier right)
        => left.Equals(right);

    public static bool operator !=(EntityAssetIdentifier left, EntityAssetIdentifier right)
        => !left.Equals(right);

    #endregion

    public override string ToString()
        => $"{{ Asset Package: {AssetPackageId}, Entity Id: {AssetId} }}";
}