using Moq;
using OSK.Petra.Assets.Internal.Models;
using OSK.Petra.Assets.Internal.Services;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using OSK.Petra.Assets.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using Xunit;

namespace OSK.Petra.Assets.UnitTests;

public class AssetServiceTests
{
    #region Variables

    private readonly Mock<IAssetManager> _mockAssetManager;
    private readonly Mock<ILoadScreen> _mockLoadScreen;
    private readonly Mock<Internal.IAssetDatabase> _mockDatabase;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly AssetService _assetService;

    #endregion

    #region Constructors

    public AssetServiceTests()
    {
        _mockAssetManager = new Mock<IAssetManager>();
        _mockLoadScreen = new Mock<ILoadScreen>();
        _mockDatabase = new Mock<Internal.IAssetDatabase>();
        _mockServiceProvider = new Mock<IServiceProvider>();

        _assetService = new AssetService(
            _mockAssetManager.Object,
            _mockLoadScreen.Object,
            _mockDatabase.Object,
            _mockServiceProvider.Object
        );
    }

    #endregion

    #region InitializeAsync

    [Fact]
    public async Task InitializeAsync_ValidOptions_InitializesSuccessfully()
    {
        // Arrange
        _mockAssetManager.Setup(m => m.InitializeDatabaseAsync(
            It.IsAny<Models.IAssetInitializationContext>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        // Act
        var result = await _assetService.InitializeAsync(
            new AssetServiceOptions { InstantiatorIdleDisposalTimeout = TimeSpan.FromMinutes(1) },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InitializeAsync_DefaultOptions_InitializesSuccessfully()
    {
        // Arrange
        _mockAssetManager.Setup(m => m.InitializeDatabaseAsync(
            It.IsAny<Models.IAssetInitializationContext>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        // Act
        var result = await _assetService.InitializeAsync(
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InitializeAsync_Error_ReturnsFailure()
    {
        // Arrange
        var errorMessage = "Initialization failed";
        _mockAssetManager.Setup(m => m.InitializeDatabaseAsync(
            It.IsAny<Models.IAssetInitializationContext>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.InvalidRequest(errorMessage));

        // Act
        var result = await _assetService.InitializeAsync(
            null,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_NoLoaders_DoesNothing()
    {
        // Act & Assert
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    #endregion

    #region InstantiateAsync

    [Fact]
    public void InstantiateAsync_NullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _assetService.InstantiateAsync<object, ITransform>(null!, CancellationToken.None);
        });

        Assert.NotNull(exception);
    }

    #endregion

    #region LoadModule

    [Fact]
    public void LoadModule_NullParameters_ThrowsArgumentNullException()
    {
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => _assetService.LoadModule((ModuleLoadParameters)null!));

        Assert.NotNull(exception);
    }

    [Fact]
    public void LoadModule_ModuleNotFound_ReturnsNull()
    {
        // Arrange
        _mockDatabase.Setup(d => d.GetModule(It.IsAny<ModuleAssetIdentifier>()))
            .Returns((IModuleDescriptor?)null);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetModuleDescriptor

    [Fact]
    public void GetModuleDescriptor_ValidIdentifier_ReturnsDescriptor()
    {
        // Arrange
        var identifier = new ModuleAssetIdentifier("test");
        var descriptor = new Mock<IModuleDescriptor>();
        _mockDatabase.Setup(d => d.GetModule(identifier))
            .Returns(descriptor.Object);

        // Act
        var actual = _assetService.GetModuleDescriptor(identifier);

        // Assert
        Assert.Equal(descriptor.Object, actual);
    }

    [Fact]
    public void GetModuleDescriptor_InvalidIdentifier_ReturnsNull()
    {
        // Arrange
        var identifier = new ModuleAssetIdentifier("test");
        var descriptor = new Mock<IModuleDescriptor>();
        _mockDatabase.Setup(d => d.GetModule(It.IsAny<ModuleAssetIdentifier>()))
            .Returns((IModuleDescriptor?)null);

        // Act
        var actual = _assetService.GetModuleDescriptor(identifier);

        // Assert
        Assert.Null(actual);
    }

    #endregion

    #region GetEntityDescriptor

    [Fact]
    public void GetEntityDescriptor_ValidIdentifier_ReturnsDescriptor()
    {
        // Arrange
        var descriptor = Mock.Of<IEntityDescriptor>();
        _mockDatabase.Setup(d => d.GetEntity(It.IsAny<EntityAssetIdentifier>()))
            .Returns(descriptor);

        // Act
        var actual = _assetService.GetEntityDescriptor(new());

        // Assert
        Assert.Equal(descriptor, actual);
    }

    [Fact]
    public void GetEntityDescriptor_InvalidIdentifier_ReturnsNull()
    {

        // Arrange
        var descriptor = Mock.Of<IEntityDescriptor>();
        _mockDatabase.Setup(d => d.GetEntity(It.IsAny<EntityAssetIdentifier>()))
            .Returns((IEntityDescriptor?)null);

        // Act
        var actual = _assetService.GetEntityDescriptor(new());

        // Assert
        Assert.Null(actual);
    }

    #endregion

    #region GetModuleDescriptors

    [Fact]
    public void GetModuleDescriptors_ReturnsAllDescriptors()
    {
        // Arrange
        var mockDescriptor1 = Mock.Of<IModuleDescriptor>();
        var mockDescriptor2 = Mock.Of<IModuleDescriptor>();

        var descriptors = new List<IModuleDescriptor>
        {
            mockDescriptor1,
            mockDescriptor2
        };
        _mockDatabase.Setup(d => d.GetModules(It.IsAny<AssetSearchOptions>()))
            .Returns(descriptors);

        // Act
        var result = _assetService.GetModuleDescriptors();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, descriptors.Count());
    }

    #endregion

    #region GetEntityDescriptors

    [Fact]
    public void GetEntityDescriptors_ReturnsAllDescriptors()
    {
        // Arrange
        var mockDescriptor1 = Mock.Of<IEntityDescriptor>();
        var mockDescriptor2 = Mock.Of<IEntityDescriptor>();

        var descriptors = new List<IEntityDescriptor>
        {
            mockDescriptor1,
            mockDescriptor2
        };
        _mockDatabase.Setup(d => d.GetEntities(It.IsAny<AssetSearchOptions>()))
            .Returns(descriptors);

        // Act
        var result = _assetService.GetEntityDescriptors();

        // Assert
        Assert.NotNull(result);
    }

    #endregion
}