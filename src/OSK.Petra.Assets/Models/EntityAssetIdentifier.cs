using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// A unique asset identifier that can be used to help with lookups in a database
/// </summary>
public readonly struct EntityAssetIdentifier : IEquatable<EntityAssetIdentifier>
{
    #region Variables

    /// <summary>
    /// The unique asset id
    /// </summary>
    public Guid AssetId { get; init; }

    /// <summary>
    /// The unique asset package the asset belongs to
    /// </summary>
    public Guid AssetPackageId { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an asset identifier that uses the provided unique id with the default package id
    /// </summary>
    /// <param name="id">The unique id for the asset</param>
    public EntityAssetIdentifier(Guid id)
        : this(AssetIdentifiers.DefaultPackageId, id)
    {
    }

    /// <summary>
    /// Creates an asset identifier that uses the provdied unique id and unique package id
    /// </summary>
    /// <param name="packageId">The package that the asset belongs to</param>
    /// <param name="id">The unique id for the asset</param>
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