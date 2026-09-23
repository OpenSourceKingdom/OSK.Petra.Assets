using System;
using OSK.Petra.Assets.Ports;

namespace OSK.Petra.Assets.Options;

/// <summary>
/// A set of options to configure how the <see cref="IAssetService"/> behaves
/// </summary>
public class AssetServiceOptions
{
    /// <summary>
    /// The amount of time an instantiator can sit idle before it is disposed a resources reclaimed
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Null indicates resource cleanup is not required and loaded instantiators will remain indefitinitely until the database is initialized again</item>
    /// </list>
    /// </remarks>
    public TimeSpan? InstantiatorIdleDisposalTimeout { get; set; }
}
