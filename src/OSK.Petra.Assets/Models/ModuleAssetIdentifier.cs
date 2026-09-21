using OSK.Petra.Modules;
using System;

namespace OSK.Petra.Assets.Models;


public readonly struct ModuleAssetIdentifier : IEquatable<ModuleAssetIdentifier>
{
    #region Variables

    public ModuleName ModuleName { get; init; }

    public Guid AssetPackageId { get; init; }

    #endregion

    #region Constructors

    public ModuleAssetIdentifier(ModuleName moduleName)
        : this(AssetIdentifiers.DefaultPackageId, moduleName)
    {
    }

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