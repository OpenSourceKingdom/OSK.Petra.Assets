using OSK.Petra.Modules;
using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// A unique module identifier that can be used to help with lookups in a database
/// </summary>
public readonly struct ModuleAssetIdentifier : IEquatable<ModuleAssetIdentifier>
{
    #region Variables

    /// <summary>
    /// A unique module name for the asset
    /// </summary>
    public ModuleName ModuleName { get; init; }

    /// <summary>
    /// The unique asset package the asset belongs to
    /// </summary>
    public Guid AssetPackageId { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a module identifier that uses the provided unique name with the default package id
    /// </summary>
    /// <param name="moduleName">The unique module name for the asset</param>
    public ModuleAssetIdentifier(ModuleName moduleName)
        : this(AssetIdentifiers.DefaultPackageId, moduleName)
    {
    }

    /// <summary>
    /// Creates a module identifier that uses the provided unique name with the provided package id
    /// </summary>
    /// <param name="packageId">The package that the asset belongs to</param>
    /// <param name="moduleName">The unique module name for the asset</param>
    public ModuleAssetIdentifier(Guid packageId, ModuleName moduleName)
    {
        AssetPackageId = packageId;
        ModuleName = moduleName;
    }

    #endregion

    #region Equality

    public bool Equals(ModuleAssetIdentifier other)
        => other.AssetPackageId == AssetPackageId && other.ModuleName.Equals(ModuleName);

    public override bool Equals(object? obj)
        => obj is ModuleAssetIdentifier other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(AssetPackageId, ModuleName);

    public static bool operator ==(ModuleAssetIdentifier left, ModuleAssetIdentifier right)
        => left.Equals(right);

    public static bool operator !=(ModuleAssetIdentifier left, ModuleAssetIdentifier right)
        => !left.Equals(right);

    #endregion

    public override string ToString()
        => $"{{ Asset Package: {AssetPackageId}, Entity Id: {ModuleName} }}";
}