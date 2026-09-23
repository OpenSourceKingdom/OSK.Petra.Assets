using OSK.Hexagonal.MetaData;
using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.Ports;

/// <summary>
/// Represents a UI visual layer load screen that is used in conjunction with the asset system, if a module requires it
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationOptional)]
public interface ILoadScreen
{
    /// <summary>
    /// Initializes and starts the load screen with the related load parameters and context
    /// </summary>
    /// <param name="loadParameters">The parameters used to load the module</param>
    /// <param name="context">The context referring to the module being loaded</param>
    void Initialize(ModuleLoadParameters loadParameters, IModuleLoadContext context);
}
