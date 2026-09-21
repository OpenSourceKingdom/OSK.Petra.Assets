using OSK.Operations.Outputs.Models;
using System.Collections.Generic;

namespace OSK.Petra.Assets.Models;

public interface IAssetInitializationContext
{
    void UpdateProgress(float progress, string? message = null);

    void AddAssets(IEnumerable<IAssetDescriptor> assetDescriptors);

    void Succeed();

    void Fail(Output error);
}
