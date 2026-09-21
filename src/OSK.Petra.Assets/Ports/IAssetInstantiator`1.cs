using OSK.Petra.Assets.Models;
using System;

namespace OSK.Petra.Assets.Ports;

public interface IAssetInstantiator<TEntity, TTransform>: IAssetInstantiator
    where TEntity: class
    where TTransform: ITransform
{
    TEntity Instantiate(TTransform transform, Action<TEntity>? configurator = null);
}
