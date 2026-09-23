using Moq;
using OSK.Expressions.Invoker.Ports;
using OSK.Petra.Assets.Internal.Models;
using OSK.Petra.Assets.Internal.Services;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using OSK.Petra.Assets.Ports;
using OSK.Operations.Outputs;
using OSK.Petra.Assets.UnitTests._Helpers;

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

        _assetService = new AssetService(_mockAssetManager.Object, _mockLoadScreen.Object, _mockDatabase.Object, _mockServiceProvider.Object);
    }

    #endregion

    #region InitializeAsync

    [Fact]
    public async Task InitializeAsync_ValidOptions_InitializesSuccessfully()
    {
        // Arrange
        _mockAssetManager.Setup(m => m.InitializeDatabaseAsync(It.IsAny<IAssetInitializationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        // Act
        var result = await _assetService.InitializeAsync(new AssetServiceOptions { InstantiatorIdleDisposalTimeout = TimeSpan.FromMinutes(1) }, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InitializeAsync_DefaultOptions_InitializesSuccessfully()
    {
        // Arrange
        _mockAssetManager.Setup(m => m.InitializeDatabaseAsync(It.IsAny<IAssetInitializationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.Success());

        // Act
        var result = await _assetService.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InitializeAsync_Error_ReturnsFailure()
    {
        // Arrange
        var errorMessage = "Initialization failed";
        _mockAssetManager.Setup(m => m.InitializeDatabaseAsync(It.IsAny<IAssetInitializationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Out.InvalidRequest(errorMessage));

        // Act
        var result = await _assetService.InitializeAsync(cancellationToken: TestContext.Current.CancellationToken);

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
        mockLoader.SetupGet(m => m.LoadProgress)
            .Returns(LoadProgress.NotStarted);

        _assetService._loaderLookup[new("Abc")] = mockLoader.Object;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        mockLoader.Verify(m => m.Update(It.IsAny<TimeSpan>()), Times.Once);
        Assert.Single(_assetService._loaderLookup);
    }

    [Fact]
    public void Update_CompletedLoader_RemovesFromLookup()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress)
            .Returns(LoadProgress.Complete);

        _assetService._loaderLookup[new("Abc")] = mockLoader.Object;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        mockLoader.Verify(m => m.Update(It.IsAny<TimeSpan>()), Times.Once);
        Assert.Empty(_assetService._loaderLookup);
    }

    [Fact]
    public void Update_FailedLoader_RemovesFromLookup()
    {
        // Arrange
        var mockLoader = new Mock<IModuleLoader>();
        mockLoader.SetupGet(m => m.LoadProgress)
            .Returns(new LoadProgress(Out.InvalidRequest("failed")));

        _assetService._loaderLookup[new("Abc")] = mockLoader.Object;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        mockLoader.Verify(m => m.Update(It.IsAny<TimeSpan>()), Times.Once);
        Assert.Empty(_assetService._loaderLookup);
    }

    [Fact]
    public void Update_MultipleLoaders_UpdatesAllAndRemovesCompleted()
    {
        // Arrange
        var mockLoader1 = new Mock<IModuleLoader>();
        mockLoader1.SetupGet(m => m.LoadProgress).Returns(LoadProgress.Complete);

        var mockLoader2 = new Mock<IModuleLoader>();
        mockLoader2.SetupGet(m => m.LoadProgress).Returns(LoadProgress.NotStarted);


        _assetService._loaderLookup[new("Abc")] = mockLoader1.Object;
        _assetService._loaderLookup[new("Def")] = mockLoader2.Object;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        mockLoader1.Verify(m => m.Update(It.IsAny<TimeSpan>()), Times.Once);
        mockLoader2.Verify(m => m.Update(It.IsAny<TimeSpan>()), Times.Once);
        Assert.Single(_assetService._loaderLookup);
    }

    [Fact]
    public void Update_IdleDisposalTimeoutDisabled_DoesNotDisposeInstantiators()
    {
        // Arrange
        var mockInstantiator = new Mock<IAssetInstantiator>();
        var entry = new EntityLookupEntry(mockInstantiator.Object, Mock.Of<IInvoker>())
        {
            LastUsed = DateTime.Now
        };

        _assetService._entryLookup[new(Guid.NewGuid())] = entry;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        Assert.Single(_assetService._entryLookup);

        mockInstantiator.Verify(m => m.Dispose(), Times.Never);
    }

    [Fact]
    public void Update_IdleInstantiatorExpired_DisposesAndRemoves()
    {
        // Arrange
        _assetService._options.InstantiatorIdleDisposalTimeout = TimeSpan.FromSeconds(1);

        var mockInstantiator = new Mock<IAssetInstantiator>();
        var entry = new EntityLookupEntry(mockInstantiator.Object, Mock.Of<IInvoker>())
        {
            LastUsed = new DateTime(2020, 1, 1)
        };
        _assetService._entryLookup[new(Guid.NewGuid())] = entry;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        Assert.Empty(_assetService._entryLookup);

        mockInstantiator.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void Update_IdleInstantiatorNotExpired_DoesNotDispose()
    {
        // Arrange
        _assetService._options.InstantiatorIdleDisposalTimeout = TimeSpan.FromSeconds(1);

        var mockInstantiator = new Mock<IAssetInstantiator>();
        var entry = new EntityLookupEntry(mockInstantiator.Object, Mock.Of<IInvoker>())
        {
            LastUsed = DateTime.Now.AddMinutes(5)
        };
        _assetService._entryLookup[new(Guid.NewGuid())] = entry;

        // Act
        _assetService.Update(TimeSpan.FromMilliseconds(100));

        // Assert
        Assert.Single(_assetService._entryLookup);

        mockInstantiator.Verify(m => m.Dispose(), Times.Never);
    }

    #endregion

    #region InstantiateAsync

    [Fact]
    public async Task InstantiateAsync_NullParameters_ThrowsArgumentNullException()
    {
        // Arraange/Act/Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _assetService.InstantiateAsync<object, ITransform>(null!, TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InstantiateAsync_CachedEntryFound_OptionsIncludeIdelTime_UpdatesLastUsed_ReturnsSuccessfully(bool useNullPreviousTime)
    {
        // Arrange
        _assetService._options.InstantiatorIdleDisposalTimeout = TimeSpan.FromSeconds(5);

        var previousUsed = DateTime.UtcNow.AddHours(-1);

        var assetId = new EntityAssetIdentifier(Guid.NewGuid());
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(1);

        var mockDescriptorInvoker = Mock.Of<IInvoker>();
        var entry = new EntityLookupEntry(mockInstantiator.Object, mockDescriptorInvoker) { LastUsed = useNullPreviousTime ? null : previousUsed };

        _assetService._entryLookup[assetId] = entry;

        var mockAssetRef = new Mock<IEntityAssetReference<ITransform>>();
        mockAssetRef.SetupGet(m => m.AssetIdentifier)
            .Returns(assetId);

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = mockAssetRef.Object,
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        if (useNullPreviousTime)
        {
            Assert.NotNull(entry.LastUsed);
        }
        else
        {
            Assert.NotEqual(previousUsed, entry.LastUsed);
        }
    }

    [Fact]
    public async Task InstantiateAsync_CachedEntryFound_OptionsIncludeIdelTime_DoesNotUpdateLastUsedTime_ReturnsSuccessfully()
    {
        // Arrange
        var assetId = new EntityAssetIdentifier(Guid.NewGuid());
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(1);

        var mockDescriptorInvoker = Mock.Of<IInvoker>();
        var entry = new EntityLookupEntry(mockInstantiator.Object, mockDescriptorInvoker) { LastUsed = null };

        _assetService._entryLookup[assetId] = entry;

        var mockAssetRef = new Mock<IEntityAssetReference<ITransform>>();
        mockAssetRef.SetupGet(m => m.AssetIdentifier)
            .Returns(assetId);

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = mockAssetRef.Object,
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Null(entry.LastUsed);
    }

    [Fact]
    public async Task InstantiateAsync_GetInstantiatorFails_ReturnsFailure()
    {
        // Arrange
        var errorMessage = "Failed to get instantiator";
        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.InvalidRequest<IAssetInstantiator<object, ITransform>>(errorMessage)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public async Task InstantiateAsync_NewInstantiator_NoServices_GetInstantiatorSuccess_ReturnsEntity()
    {
        // Arrange
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        var mockEntity = new Mock<object>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(mockEntity.Object);

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(It.IsAny<IEntityAssetReference<ITransform>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success(mockInstantiator.Object)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task InstantiateAsync_WithServices_InstantiatorNotCached_CachesAndInjectsDependencies_ReturnsSuccessfully()
    {
        // Arrange
        var testEntity = new TestEntity()
        {
            ServiceProvider = null!
        };
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns(testEntity);

        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns((ITransform _, Action<object> action) =>
            {
                action(testEntity);
                return testEntity;
            });

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success(mockInstantiator.Object)));

        var mockServices = new Mock<IServiceProvider>();
        mockServices.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IServiceProvider))))
            .Returns(mockServices.Object);

        var identifier = new EntityAssetIdentifier(Guid.NewGuid());
        var mockReference = new Mock<IEntityAssetReference<ITransform>>();
        mockReference.SetupGet(m => m.AssetIdentifier)
            .Returns(identifier);

        var mockDescriptor = new Mock<IEntityDescriptor>();
        _mockDatabase.Setup(d => d.GetEntity(identifier))
            .Returns(mockDescriptor.Object);

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = mockReference.Object,
            Transform = Mock.Of<ITransform>(),
            Services = mockServices.Object
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(mockServices.Object, testEntity.ServiceProvider);

        Assert.Single(_assetService._entryLookup);
        Assert.Equal(_assetService._entryLookup[identifier].Instantiator, mockInstantiator.Object);
    }

    [Fact]
    public async Task InstantiateAsync_NoAssetIdentifier_CreatesNewInstantiator()
    {
        // Arrange
        var mockInstantiator = new Mock<IAssetInstantiator<object, ITransform>>();
        var mockEntity = new Mock<object>();
        mockInstantiator.Setup(m => m.Instantiate(It.IsAny<ITransform>(), It.IsAny<Action<object>>()))
            .Returns((ITransform _, Action<object> action) =>
            {
                action(mockEntity.Object);
                return mockEntity.Object;
            });

        _mockAssetManager.Setup(m => m.GetInstantiatorAsync<object, ITransform>(
            It.IsAny<IEntityAssetReference<ITransform>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Out.Success(mockInstantiator.Object)));

        var parameters = new InstantiationParameters<ITransform>
        {
            AssetReference = Mock.Of<IEntityAssetReference<ITransform>>(r => r.AssetIdentifier == null),
            Transform = Mock.Of<ITransform>(),
            Services = null
        };

        // Act
        var result = await _assetService.InstantiateAsync<object, ITransform>(parameters, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Empty(_assetService._entryLookup);
    }

    #endregion

    #region LoadModule

    [Fact]
    public void LoadModule_NullParameters_ThrowsArgumentNullException()
    {
        // Arrange/Act/Assert
        Assert.Throws<ArgumentNullException>(() => _assetService.LoadModule((ModuleLoadParameters)null!));
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
        _assetService._loaderLookup[moduleIdentifier] = mockLoader.Object;

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
        Assert.Equal(mockLoader.Object, result);
    }

    [Fact]
    public void LoadModule_LoaderTypeNotAValidLoader_ReturnsNull()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier("test");
        var mockDescriptor = new Mock<IModuleDescriptor>();
        mockDescriptor.SetupGet(m => m.AssetIdentifier).Returns(moduleIdentifier);

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        mockDescriptor.Setup(m => m.GetModuleLoaderType()).Returns(typeof(ModuleLoadParameters));

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test"));

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LoadModule_ReplaceBehavior_CallsLoadScreenInitialize(bool useLoadScreen)
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier("test");
        var mockDescriptor = new Mock<IModuleDescriptor>();
        mockDescriptor.SetupGet(m => m.AssetIdentifier)
            .Returns(moduleIdentifier);

        mockDescriptor.Setup(m => m.GetModuleLoaderType())
            .Returns(typeof(TestLoader));

        _mockDatabase.Setup(d => d.GetModule(moduleIdentifier))
            .Returns(mockDescriptor.Object);

        // Act
        var result = _assetService.LoadModule(new ModuleLoadParameters("test")
        {
            LoadBehavior = useLoadScreen ? ModuleLoadBehavior.Replace : ModuleLoadBehavior.Additive
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(TestLoader), result.GetType());

        _mockLoadScreen.Verify(m => m.Initialize(It.IsAny<ModuleLoadParameters>(), It.IsAny<IModuleLoadContext>()), useLoadScreen ? Times.Once : Times.Never);
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
        _mockDatabase.Verify(m => m.GetModule(It.IsAny<ModuleAssetIdentifier>()), Times.Once);
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
        _mockDatabase.Verify(m => m.GetModule(It.IsAny<ModuleAssetIdentifier>()), Times.Once);
    }

    #endregion

    #region GetEntityDescriptor

    [Fact]
    public void GetEntityDescriptor_CallsDatabase_ReturnsExpected()
    {
        // Arrange
        var descriptor = Mock.Of<IEntityDescriptor>();
        _mockDatabase.Setup(d => d.GetEntity(It.IsAny<EntityAssetIdentifier>()))
            .Returns(descriptor);

        // Act
        var actual = _assetService.GetEntityDescriptor(new());

        // Assert
        Assert.Equal(descriptor, actual);
        _mockDatabase.Verify(m => m.GetEntity(It.IsAny<EntityAssetIdentifier>()), Times.Once);
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
        _mockDatabase.Verify(m => m.GetEntity(It.IsAny<EntityAssetIdentifier>()), Times.Once);
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
        _mockDatabase.Setup(d => d.GetModules(It.IsAny<AssetSearchOptions?>()))
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
        _mockDatabase.Setup(d => d.GetModules(It.IsAny<AssetSearchOptions?>()))
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
        _mockDatabase.Setup(d => d.GetEntities(It.IsAny<AssetSearchOptions?>()))
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
        _mockDatabase.Setup(d => d.GetEntities(It.IsAny<AssetSearchOptions?>()))
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
