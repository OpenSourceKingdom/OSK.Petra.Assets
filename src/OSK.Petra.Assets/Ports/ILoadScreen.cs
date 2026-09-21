using OSK.Hexagonal.MetaData;
using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Ports;

[HexagonalIntegration(HexagonalIntegrationType.IntegrationOptional)]
public interface ILoadScreen
{
    void Initialize(ModuleLoadParameters loadParameters, IModuleLoadContext context);
}
