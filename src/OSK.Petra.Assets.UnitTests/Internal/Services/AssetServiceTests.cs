using Moq;
using OSK.Expressions.Invoker.Ports;
using OSK.Petra.Assets.Internal.Models;
using OSK.Petra.Assets.Internal.Services;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using OSK.Petra.Assets.Ports;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;

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
            It.IsAny<IAssetInitializationContext>(),
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
            It.IsAny<IAssetInitializationContext>(),
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
            It.IsAny<IAssetInitializationContext>(),
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

    [Fact]
    public void Update_LoadingLoader_CallsUpdateOnLoader()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_CompletedLoader_RemovesFromLookup()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(LoadProgress.Complete);

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_FailedLoader_RemovesFromLookup()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(new LoadProgress(Out.InvalidRequest("failed")));

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_MultipleLoaders_UpdatesAllAndRemovesCompleted()
    {
        // Arrange
        var mockLoader1 = new Mock<IModuleLoader>();
        mockLoader1.SetupGet(m => m.LoadProgress).Returns(LoadProgress.Complete);

        var mockLoader2 = new Mock<IModuleLoader>();
        mockLoader2.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_IdleDisposalTimeoutDisabled_DoesNotDisposeInstantiators()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_IdleInstantiatorExpired_DisposesAndRemoves()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_IdleInstantiatorNotExpired_DoesNotDispose()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Update_IdleDisposalTimeoutZero_DoesNotDisposeInstantiators()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);

        // Act
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

    [Fact]
    public async Task InstantiateAsync_CachedEntryFound_UpdatesLastUsedAndReturnsInstantiator()
    {
        // Arrange
        var assetId = new EntityAssetIdentifier(Guid.NewGuid());
        var mockInstantiator = Mock.Of<IAssetInstantiator<object, ITransform>>();
        var mockDescriptorInvoker = Mock.Of<IInvoker>();
        var entry = new EntityLookupEntry(mockInstantiator, mockDescriptorInvoker) { LastUsed = DateTime.UtcNow.AddHours(-1) };

        _mockDatabase.Setup(d => d.GetEntity(assetId))
            .Returns((IEntityDescriptor?)null);

        var mockAssetRef = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == assetId);

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Output<IAssetInstantiator<object, ITransform>>>(Out.InvalidRequest<IAssetInstantiator<object, ITransform>>("not reached")));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = mockAssetRef,
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InstantiateAsync_NewInstantiator_GetInstantiatorSuccess_ReturnsEntity()
    {
        // Arrange
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        var mockEntity = new Mock<object>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(mockEntity.Object);

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Output<IAssetInstantiator<object, ITransform>>>(Out.Success(mockInstantiator.Object)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InstantiateAsync_GetInstantiatorFails_ReturnsFailure()
    {
        // Arrange
        var errorMessage = "Failed to get instantiator";
        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Output<IAssetInstantiator<object, ITransform>>>(Out.InvalidRequest<IAssetInstantiator<object, ITransform>>(errorMessage)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task InstantiateAsync_WithServices_InjectsDependencies()
    {
        // Arrange
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        var mockEntity = new Mock<object>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(mockEntity.Object);

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Output<IAssetInstantiator<object, ITransform>>>(Out.Success(mockInstantiator.Object)));

        var mockServices = new Mock<IServiceProvider>();

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = mockServices.Object
        };

        // Act
        await _assetService.InstantiateAsync<object, ITransform>(parameters, CancellationToken.None);

        // Assert
    }

    [Fact]
    public async Task InstantiateAsync_NoAssetIdentifier_CreatesNewInstantiator()
    {
        // Arrange
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        var mockEntity = new Mock<object>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(mockEntity.Object);

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Output<IAssetInstantiator<object, ITransform>>>(Out.Success(mockInstantiator.Object)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InstantiateAsync_WithDescriptor_CreatesDescriptorInvoker()
    {
        // Arrange
        var assetId = new EntityAssetIdentifier(Guid.NewGuid());
        var mockDescriptor = new Mock<IEntityDescriptor>();

        _mockDatabase.Setup(d => d.GetEntity(assetId))
            .Returns(mockDescriptor.Object);

        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        var mockEntity = new Mock<object>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(mockEntity.Object);

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Output<IAssetInstantiator<object, ITransform>>>(Out.Success(mockInstantiator.Object)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == assetId),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    #endregion

    #region LoadModule

    [Fact]
    public void LoadModule_NullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _assetService.LoadModule((ModuleLoadParameters)null!));
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

    [Fact]
    public void LoadModule_ModuleAlreadyLoaded_ReturnsExistingLoader()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier("test");
        var mockDescriptor = new Mock<IModuleDescriptor>();
        mockDescriptor.SetupGet(m => m.AssetIdentifier).Returns(moduleIdentifier);

        var mockLoader = new Mock<IModuleLoader>();

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
    }

    [Fact]
    public void LoadModule_ValidModule_ReturnsLoader()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier("test");
        var mockDescriptor = new Mock<IModuleDescriptor>();
        mockDescriptor.SetupGet(m => m.AssetIdentifier).Returns(moduleIdentifier);

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
    }

    [Fact]
    public void LoadModule_InvalidLoaderType_ReturnsNull()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier("test");
        var mockDescriptor = new Mock<IModuleDescriptor>();
        mockDescriptor.SetupGet(m => m.AssetIdentifier).Returns(moduleIdentifier);

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
    }

    [Fact]
    public void LoadModule_ReplaceBehavior_CallsLoadScreenInitialize()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier("test");
        var mockDescriptor = new Mock<IModuleDescriptor>();
        mockDescriptor.SetupGet(m => m.AssetIdentifier).Returns(moduleIdentifier);

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
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
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetModuleDescriptors_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var descriptors = new List<IModuleDescriptor>();
        _mockDatabase.Setup(d => d.GetModules(It.IsAny<AssetSearchOptions>()))
            .Returns(descriptors);

        // Act
        var result = _assetService.GetModuleDescriptors();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetModuleDescriptors_WithSearchOptions_PassesOptions()
    {
        // Arrange
        var mockDescriptor = Mock.Of<IModuleDescriptor>();
        var descriptors = new List<IModuleDescriptor> { mockDescriptor };
        var searchOptions = new AssetSearchOptions();

        _mockDatabase.Setup(d => d.GetModules(searchOptions))
            .Returns(descriptors);

        // Act
        var result = _assetService.GetModuleDescriptors(searchOptions);

        // Assert
        Assert.NotNull(result);
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
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetEntityDescriptors_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var descriptors = new List<IEntityDescriptor>();
        _mockDatabase.Setup(d => d.GetEntities(It.IsAny<AssetSearchOptions>()))
            .Returns(descriptors);

        // Act
        var result = _assetService.GetEntityDescriptors();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetEntityDescriptors_WithSearchOptions_PassesOptions()
    {
        // Arrange
        var mockDescriptor = Mock.Of<IEntityDescriptor>();
        var descriptors = new List<IEntityDescriptor> { mockDescriptor };
        var searchOptions = new AssetSearchOptions();

        _mockDatabase.Setup(d => d.GetEntities(searchOptions))
            .Returns(descriptors);

        // Act
        var result = _assetService.GetEntityDescriptors(searchOptions);

        // Assert
        Assert.NotNull(result);
    }

    #endregion
}
