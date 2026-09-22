using System;

namespace OSK.Petra.Assets.Models;

/// <summary>
/// Parameters used to instantiate an entity asset
/// </summary>
/// <typeparam name="TTransform">The transform that the entity utilizes</typeparam>
public class InstantiationParameters<TTransform>()
    where TTransform: ITransform
{
    /// <summary>
    /// The reference to use to instantiate the entity
    /// </summary>
    public required IEntityAssetReference<TTransform> AssetReference { get; init; }

    /// <summary>
    /// The transform information to apply to the entity
    /// </summary>
    public required TTransform Transform { get; init; }

    /// <summary>
    /// Services used to initialize the entity after it is instantiated
    /// </summary>
    public IServiceProvider? Services { get; init; }
}
