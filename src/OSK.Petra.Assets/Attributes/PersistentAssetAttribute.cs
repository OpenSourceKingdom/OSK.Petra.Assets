using System;

namespace OSK.Petra.Assets.Attributes;

/// <summary>
/// Marks a game asset as persistent, indicating it should persist across scene changes and not be destroyed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PersistentAssetAttribute : Attribute
{
}

