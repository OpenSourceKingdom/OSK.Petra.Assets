using OSK.Petra.DependencyInjection.Attributes;

namespace OSK.Petra.Assets.UnitTests._Helpers;

public class TestEntity
{
    [Inject]
    public IServiceProvider ServiceProvider { get; set; }
}
