using System;

namespace OSK.Petra.Assets.Models;

public class InstantiationParameters<TTransform>()
    where TTransform: ITransform
{
    public required IEntityAssetReference<TTransform> AssetReference { get; init; }

    public required TTransform Transform { get; init; }

    public IServiceProvider? Services { get; init; }
}
