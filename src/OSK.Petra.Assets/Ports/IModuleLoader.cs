using OSK.Hexagonal.MetaData;
using OSK.Petra.Assets.Models;
using System;

namespace OSK.Petra.Assets.Ports;

/// <summary>
/// A loader that is capable of setting up and loading a module
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.IntegrationRequired)]
public interface IModuleLoader: IModuleLoadContext
{
    /// <summary>
    /// Updates the loader to perform load operations over the course of several frames
    /// </summary>
    /// <param name="deltaTime">The amount of time since the last frame</param>
    void Update(TimeSpan deltaTime);
}
