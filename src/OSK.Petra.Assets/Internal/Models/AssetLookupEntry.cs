using System;
using OSK.Expressions.Invoker.Ports;
using OSK.Petra.Assets.Ports;

namespace OSK.Petra.Assets.Internal.Models;

internal class EntityLookupEntry(IAssetInstantiator instantiator, IInvoker? descriptorInvoker)
{
    public IAssetInstantiator Instantiator => instantiator;

    public IInvoker? DescriptorInvoker => descriptorInvoker;

    public DateTime? LastUsed { get; set; }
}
