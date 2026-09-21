using OSK.Expressions.Invoker.Ports;
using OSK.Petra.Assets.Ports;
using System;

namespace OSK.Petra.Assets.Internal.Models;

internal class EntityLookupEntry(IAssetInstantiator instantiator, IInvoker? descriptorInvoker)
{
    public IAssetInstantiator Instantiator => instantiator;

    public IInvoker? DescriptorInvoker => descriptorInvoker;

    public DateTime? LastUsed { get; set; }
}
