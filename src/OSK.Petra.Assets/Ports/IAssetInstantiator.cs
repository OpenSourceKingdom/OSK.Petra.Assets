using OSK.Hexagonal.MetaData;
using System;

namespace OSK.Petra.Assets.
    Ports;
/// <summary>
/// Represents an instantiator for a specific entity
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface IAssetInstantiator: IDisposable
{
}
