using Moq;
using OSK.Petra.Assets.Events;
using OSK.Petra.Assets.Internal.Services;
using OSK.Petra.Assets.Models;
using OSK.Petra.Assets.Options;
using OSK.Operations.Outputs;
using OSK.Petra.Modules;

namespace OSK.Petra.Assets.UnitTests;

public class AssetDatabaseTests
{
    #region Variables

    private readonly AssetDatabase _database;

    #endregion

    #region Constructors

    public AssetDatabaseTests()
    {
        _database = new AssetDatabase();
    }

    #endregion

    #region StartInitialization

    [Fact]
    public void StartInitialization_ClearsAllDescriptors()
    {
        // Arrange
        var entityDescriptor = new Mock<IEntityDescriptor>();
        entityDescriptor.SetupGet(e => e.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));
        var moduleDescriptor = new Mock<IModuleDescriptor>();
        moduleDescriptor.SetupGet(m => m.AssetIdentifier).Returns(new ModuleAssetIdentifier(new ModuleName("test")));

        _database.StartInitialization();
        _database.AddAssets([entityDescriptor.Object, moduleDescriptor.Object]);
        _database.Succeed();

        _database.StartInitialization();

        // Assert
        Assert.Equal(0, _database.TotalEntities);
        Assert.Equal(0, _database.TotalModules);
    }

    [Fact]
    public void StartInitialization_FiresStartedEvent()
    {
        // Arrange
        AssetDatabaseEvent? capturedEvent = null;
        _database.DatabaseEvent += e => capturedEvent = e;

        // Act
        _database.StartInitialization();

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.IsType<DatabaseInitializationStartedEvent>(capturedEvent!);
    }

    [Fact]
    public void StartInitialization_SetsProgressMessage()
    {
        // Arrange & Act
        _database.StartInitialization();

        // Assert
        Assert.Contains("Initializing Database", _database.InitializationProgress.Messaage);
    }

    #endregion

    #region AddAssets

    [Fact]
    public void AddAssets_NotInProgress_DoesNotAddDescriptors()
    {
        // Arrange
        var descriptor = new Mock<IEntityDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));

        // Act
        _database.AddAssets([descriptor.Object]);

        // Assert
        Assert.Equal(0, _database.TotalEntities);
    }

    [Fact]
    public void AddAssets_EntityDescriptor_AddsToEntityLookup()
    {
        // Arrange
        var assetId = new EntityAssetIdentifier(Guid.NewGuid());
        var descriptor = new Mock<IEntityDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(assetId);

        _database.StartInitialization();

        // Act
        _database.AddAssets([descriptor.Object]);

        // Assert
        Assert.Equal(1, _database.TotalEntities);
        Assert.NotNull(_database.GetEntity(assetId));
    }

    [Fact]
    public void AddAssets_ModuleDescriptor_AddsToModuleLookup()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier(new ModuleName("test"));
        var descriptor = new Mock<IModuleDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(moduleIdentifier);

        _database.StartInitialization();

        // Act
        _database.AddAssets([descriptor.Object]);

        // Assert
        Assert.Equal(1, _database.TotalModules);
        Assert.NotNull(_database.GetModule(moduleIdentifier));
    }

    [Fact]
    public void AddAssets_MixedDescriptors_AddsToCorrectLookup()
    {
        // Arrange
        var entityDescriptor = new Mock<IEntityDescriptor>();
        entityDescriptor.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));

        var moduleDescriptor = new Mock<IModuleDescriptor>();
        moduleDescriptor.SetupGet(d => d.AssetIdentifier).Returns(new ModuleAssetIdentifier(new ModuleName("test")));

        _database.StartInitialization();

        // Act
        _database.AddAssets([entityDescriptor.Object, moduleDescriptor.Object]);

        // Assert
        Assert.Equal(1, _database.TotalEntities);
        Assert.Equal(1, _database.TotalModules);
    }

    [Fact]
    public void AddAssets_Succeeds_DoesNotAddDescriptors()
    {
        // Arrange
        var descriptor = new Mock<IEntityDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));

        _database.StartInitialization();
        _database.Succeed();

        // Act
        _database.AddAssets([descriptor.Object]);

        // Assert
        Assert.Equal(0, _database.TotalEntities);
    }

    [Fact]
    public void AddAssets_Failed_DoesNotAddDescriptors()
    {
        // Arrange
        var descriptor = new Mock<IEntityDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));

        _database.StartInitialization();
        _database.Fail(Out.InvalidRequest("test error"));

        // Act
        _database.AddAssets([descriptor.Object]);

        // Assert
        Assert.Equal(0, _database.TotalEntities);
    }

    [Fact]
    public void AddAssets_DuplicateEntityIdentifier_OverridesOriginal()
    {
        // Arrange
        var assetId = new EntityAssetIdentifier(Guid.NewGuid());
        var descriptor1 = new Mock<IEntityDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(assetId);

        var descriptor2 = new Mock<IEntityDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(assetId);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object]);

        // Act
        _database.AddAssets([descriptor2.Object]);

        // Assert
        Assert.Same(descriptor2.Object, _database.GetEntity(assetId));
    }

    [Fact]
    public void AddAssets_DuplicateModuleIdentifier_OverridesOriginal()
    {
        // Arrange
        var moduleIdentifier = new ModuleAssetIdentifier(new ModuleName("test"));
        var descriptor1 = new Mock<IModuleDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(moduleIdentifier);

        var descriptor2 = new Mock<IModuleDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(moduleIdentifier);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object]);

        // Act
        _database.AddAssets([descriptor2.Object]);

        // Assert
        Assert.Same(descriptor2.Object, _database.GetModule(moduleIdentifier));
    }

    #endregion

    #region Succeed

    [Fact]
    public void Succeed_SetsProgressToComplete_FiresEvent()
    {
        // Arrange
        AssetDatabaseEvent? capturedEvent = null;
        _database.DatabaseEvent += e => capturedEvent = e;
        _database.StartInitialization();

        // Act
        _database.Succeed();

        // Assert
        Assert.Equal(ProgressState.Complete, _database.InitializationState);
        Assert.True(_database.InitializationProgress.IsReady);
        Assert.Equal(1, _database.InitializationProgress.Percentage);

        Assert.NotNull(capturedEvent);
        Assert.IsType<DatabaseInitializationCompleteEvent>(capturedEvent!);
    }

    #endregion

    #region Fail

    [Fact]
    public void Fail_SetsProgressToFailed_ClearsDescriptors_FiresEvent()
    {
        // Arrange
        AssetDatabaseEvent? capturedEvent = null;
        _database.DatabaseEvent += e => capturedEvent = e;

        var entityDescriptor = new Mock<IEntityDescriptor>();
        entityDescriptor.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));
        var moduleDescriptor = new Mock<IModuleDescriptor>();
        moduleDescriptor.SetupGet(d => d.AssetIdentifier).Returns(new ModuleAssetIdentifier(new ModuleName("test")));

        var error = Out.InvalidRequest("test error");
        _database.StartInitialization();

        // Act
        _database.AddAssets([entityDescriptor.Object, moduleDescriptor.Object]);
        _database.Fail(error);

        // Assert
        Assert.Equal(ProgressState.Failed, _database.InitializationState);
        Assert.Contains(_database.InitializationProgress.Messaage, error.ErrorInformation.Messages);

        Assert.True(_database.InitializationProgress.IsFinished);
        Assert.False(_database.InitializationProgress.IsReady);

        Assert.Equal(0, _database.TotalEntities);
        Assert.Equal(0, _database.TotalModules);

        Assert.NotNull(capturedEvent);
        var failedEvent = Assert.IsType<DatabaseInitializationFailedEvent>(capturedEvent!);
    }

    [Fact]
    public void Fail_WithSuccessfulOutput_ThrowsInvalidOperationException()
    {
        // Arrange
        var db = new AssetDatabase();
        db.StartInitialization();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => db.Fail(Out.Success()));
    }

    #endregion

    #region GetModule

    [Fact]
    public void GetModule_ExistingIdentifier_ReturnsDescriptor()
    {
        // Arrange
        var identifier = new ModuleAssetIdentifier(new ModuleName("test"));
        var descriptor = new Mock<IModuleDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(identifier);

        _database.StartInitialization();
        _database.AddAssets([descriptor.Object]);

        // Act
        var result = _database.GetModule(identifier);

        // Assert
        Assert.Same(descriptor.Object, result);
    }

    [Fact]
    public void GetModule_NonExistingIdentifier_ReturnsNull()
    {
        // Arrange
        var identifier = new ModuleAssetIdentifier(new ModuleName("nonexistent"));

        // Act
        var result = _database.GetModule(identifier);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetModule_BeforeInitialization_ReturnsNull()
    {
        // Arrange
        var identifier = new ModuleAssetIdentifier(new ModuleName("test"));

        // Act
        var result = _database.GetModule(identifier);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetEntity

    [Fact]
    public void GetEntity_ExistingIdentifier_ReturnsDescriptor()
    {
        // Arrange
        var identifier = new EntityAssetIdentifier(Guid.NewGuid());
        var descriptor = new Mock<IEntityDescriptor>();
        descriptor.SetupGet(d => d.AssetIdentifier).Returns(identifier);

        _database.StartInitialization();
        _database.AddAssets([descriptor.Object]);

        // Act
        var result = _database.GetEntity(identifier);

        // Assert
        Assert.Same(descriptor.Object, result);
    }

    [Fact]
    public void GetEntity_NonExistingIdentifier_ReturnsNull()
    {
        // Arrange
        var identifier = new EntityAssetIdentifier(Guid.NewGuid());

        // Act
        var result = _database.GetEntity(identifier);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEntity_BeforeInitialization_ReturnsNull()
    {
        // Arrange
        var identifier = new EntityAssetIdentifier(Guid.NewGuid());

        // Act
        var result = _database.GetEntity(identifier);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetModules

    [Fact]
    public void GetModules_NotInitialized_ReturnsEmpty()
    {
        // Arrange & Act
        var result = _database.GetModules();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetModules_Initialized_ReturnsAllDescriptors()
    {
        // Arrange
        var descriptor1 = new Mock<IModuleDescriptor>();
        descriptor1.Setup(m => m.AssetIdentifier)
            .Returns(new ModuleAssetIdentifier("Abc"));

        var descriptor2 = new Mock<IModuleDescriptor>();
        descriptor2.Setup(m => m.AssetIdentifier)
            .Returns(new ModuleAssetIdentifier("Def"));

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        // Act
        var result = _database.GetModules();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetModules_EmptyAfterSucceed_ReturnsEmpty()
    {
        // Arrange
        _database.StartInitialization();
        _database.Succeed();

        // Act
        var result = _database.GetModules();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetModules_WithPackageIdFilter_FiltersByPackage()
    {
        // Arrange
        var packageId = Guid.NewGuid();
        var descriptor1 = new Mock<IModuleDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(new ModuleAssetIdentifier(packageId, new ModuleName("mod1")));

        var descriptor2 = new Mock<IModuleDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(new ModuleAssetIdentifier(Guid.NewGuid(), new ModuleName("mod2")));

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions().WithPackageIds(packageId);

        // Act
        var result = _database.GetModules(searchOptions);

        // Assert
        Assert.Single(result);
        Assert.Same(descriptor1.Object, result.First());
    }

    [Fact]
    public void GetModules_WithTagFilter_AnyBehavior_ReturnsMatching()
    {
        // Arrange
        var descriptor1 = new Mock<IModuleDescriptor>();
        descriptor1.Setup(m => m.AssetIdentifier)
            .Returns(new ModuleAssetIdentifier("Abc"));
        descriptor1.SetupGet(d => d.Tags).Returns([new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity"))]);

        var descriptor2 = new Mock<IModuleDescriptor>();
        descriptor2.Setup(m => m.AssetIdentifier)
            .Returns(new ModuleAssetIdentifier("Def"));
        descriptor2.SetupGet(d => d.Tags).Returns([new AssetTag(new AssetTagCategory("type"), new AssetTagValue("module"))]);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions()
            .WithTagFilter(new AssetTagFilter("type", [new AssetTagValue("entity")]));

        // Act
        var result = _database.GetModules(searchOptions);

        // Assert
        Assert.Single(result);
        Assert.Same(descriptor1.Object, result.First());
    }

    [Fact]
    public void GetModules_WithTagFilter_AllBehavior_ReturnsMatching()
    {
        // Arrange
        var descriptor1 = new Mock<IModuleDescriptor>();
        descriptor1.Setup(m => m.AssetIdentifier)
            .Returns(new ModuleAssetIdentifier("Abc"));
        descriptor1.SetupGet(d => d.Tags).Returns([
            new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity")),
            new AssetTag(new AssetTagCategory("region"), new AssetTagValue("spawn"))
        ]);

        var descriptor2 = new Mock<IModuleDescriptor>();
        descriptor2.Setup(m => m.AssetIdentifier)
            .Returns(new ModuleAssetIdentifier("Def"));
        descriptor2.SetupGet(d => d.Tags).Returns([
            new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity")),
            new AssetTag(new AssetTagCategory("region"), new AssetTagValue("despawn"))
        ]);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions()
            .WithTagFilter(new AssetTagFilter("type", [new AssetTagValue("entity")]))
            .WithTagFilter(new AssetTagFilter("region", [new AssetTagValue("spawn")]));

        // Act
        var result = _database.GetModules(searchOptions);

        // Assert
        Assert.Single(result);
        Assert.Same(descriptor1.Object, result.First());
    }

    [Fact]
    public void GetModules_WithPackageIdsAndTagFilters_AppiesBothFilters()
    {
        // Arrange
        var packageId = Guid.NewGuid();
        var descriptor1 = new Mock<IModuleDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(new ModuleAssetIdentifier(packageId, new ModuleName("mod1")));
        descriptor1.SetupGet(d => d.Tags).Returns([new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity"))]);

        var descriptor2 = new Mock<IModuleDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(new ModuleAssetIdentifier(packageId, new ModuleName("mod2")));
        descriptor2.SetupGet(d => d.Tags).Returns([new AssetTag(new AssetTagCategory("type"), new AssetTagValue("module"))]);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions
        {
            AssetPackageIds = [packageId],
            TagFilters = [new AssetTagFilter("type", [new AssetTagValue("entity")])]
        };

        // Act
        var result = _database.GetModules(searchOptions);

        // Assert
        Assert.Single(result);
        Assert.Same(descriptor1.Object, result.First());
    }

    #endregion

    #region GetEntities

    [Fact]
    public void GetEntities_NotInitialized_ReturnsEmpty()
    {
        // Arrange & Act
        var result = _database.GetEntities();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetEntities_Initialized_ReturnsAllDescriptors()
    {
        // Arrange
        var descriptor1 = new Mock<IEntityDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));

        var descriptor2 = new Mock<IEntityDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        // Act
        var result = _database.GetEntities();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetEntities_EmptyAfterSucceed_ReturnsEmpty()
    {
        // Arrange
        _database.StartInitialization();
        _database.Succeed();

        // Act
        var result = _database.GetEntities();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetEntities_WithPackageIdFilter_FiltersByPackage()
    {
        // Arrange
        var packageId = Guid.NewGuid();
        var descriptor1 = new Mock<IEntityDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(packageId, Guid.NewGuid()));

        var descriptor2 = new Mock<IEntityDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid(), Guid.NewGuid()));

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions().WithPackageIds(packageId);

        // Act
        var result = _database.GetEntities(searchOptions);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void GetEntities_WithTagFilter_AnyBehavior_ReturnsMatching()
    {
        // Arrange
        var descriptor1 = new Mock<IEntityDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));
        descriptor1.SetupGet(d => d.Tags).Returns([new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity"))]);

        var descriptor2 = new Mock<IEntityDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));
        descriptor2.SetupGet(d => d.Tags).Returns([new AssetTag(new AssetTagCategory("type"), new AssetTagValue("module"))]);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions()
            .WithTagFilter(new AssetTagFilter("type", [new AssetTagValue("entity")], AssetTagFilterCondition.Any));

        // Act
        var result = _database.GetEntities(searchOptions);

        // Assert
        Assert.Single(result);
        Assert.Same(descriptor1.Object, result.First());
    }

    [Fact]
    public void GetEntities_WithTagFilter_AllBehavior_ReturnsMatching()
    {
        // Arrange
        var descriptor1 = new Mock<IEntityDescriptor>();
        descriptor1.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));
        descriptor1.SetupGet(d => d.Tags).Returns([
            new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity")),
            new AssetTag(new AssetTagCategory("region"), new AssetTagValue("spawn"))
        ]);

        var descriptor2 = new Mock<IEntityDescriptor>();
        descriptor2.SetupGet(d => d.AssetIdentifier).Returns(new EntityAssetIdentifier(Guid.NewGuid()));
        descriptor2.SetupGet(d => d.Tags).Returns([
            new AssetTag(new AssetTagCategory("type"), new AssetTagValue("entity")),
            new AssetTag(new AssetTagCategory("region"), new AssetTagValue("despawn"))
        ]);

        _database.StartInitialization();
        _database.AddAssets([descriptor1.Object, descriptor2.Object]);
        _database.Succeed();

        var searchOptions = new AssetSearchOptions()
            .WithTagFilter(new AssetTagFilter("type", [new AssetTagValue("entity")]))
            .WithTagFilter(new AssetTagFilter("region", [new AssetTagValue("spawn")]));

        // Act
        var result = _database.GetEntities(searchOptions);

        // Assert
        Assert.Single(result);
        Assert.Same(descriptor1.Object, result.First());
    }

    #endregion

    #region InitializationState

    [Fact]
    public void InitializationState_InitialDefault_NotStarted_ReturnsNotStarted()
    {
        // Arrange/Act/Assert
        Assert.Equal(ProgressState.NotStarted, _database.InitializationState);
    }

    [Fact]
    public void InitializationState_AfterStart_ReturnsInProgress()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        var state = _database.InitializationState;

        // Assert
        Assert.Equal(ProgressState.InProgress, state);
    }

    [Fact]
    public void InitializationState_AfterSucceed_ReturnsComplete()
    {
        // Arrange
        _database.StartInitialization();
        _database.Succeed();

        // Act
        var state = _database.InitializationState;

        // Assert
        Assert.Equal(ProgressState.Complete, state);
    }

    [Fact]
    public void InitializationState_AfterFail_ReturnsFailed()
    {
        // Arrange
        _database.StartInitialization();
        _database.Fail(Out.InvalidRequest("error"));

        // Act
        var state = _database.InitializationState;

        // Assert
        Assert.Equal(ProgressState.Failed, state);
    }

    #endregion

    #region UpdateProgress

    [Fact]
    public void UpdateProgress_SetsProgressValue()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(0.75f, "Loading assets");

        // Assert
        Assert.Equal(ProgressState.InProgress, _database.InitializationProgress.State);
        Assert.Equal(0.75, _database.InitializationProgress.Percentage);
    }

    [Fact]
    public void UpdateProgress_SetsMessage()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(0.5f, "Custom message");

        // Assert
        Assert.Contains("Custom message", _database.InitializationProgress.Messaage);
    }

    [Fact]
    public void UpdateProgress_NullMessage_UsesDefaultFormat()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(0.5f, null!);

        // Assert
        Assert.Contains("Loading", _database.InitializationProgress.Messaage);
    }

    [Fact]
    public void UpdateProgress_EmptyMessage_UsesDefaultFormat()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(0.5f, "");

        // Assert
        Assert.Contains("Loading", _database.InitializationProgress.Messaage);
    }

    [Fact]
    public void UpdateProgress_WhitespaceMessage_UsesDefaultFormat()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(0.5f, "   ");

        // Assert
        Assert.Contains("Loading", _database.InitializationProgress.Messaage);
    }

    [Fact]
    public void UpdateProgress_FullProgress_SetsCompleteState()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(1.0f);

        // Assert
        Assert.Equal(ProgressState.Complete, _database.InitializationProgress.State);
    }

    [Fact]
    public void UpdateProgress_OverOne_PercantageClampedToOne()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(1.5f);

        // Assert
        Assert.Equal(1, _database.InitializationProgress.Percentage);
        Assert.Equal(ProgressState.Complete, _database.InitializationProgress.State);
    }

    [Fact]
    public void UpdateProgress_Negative_PercantageClampedToZero()
    {
        // Arrange
        _database.StartInitialization();

        // Act
        _database.UpdateProgress(-0.5f);

        // Assert
        Assert.Equal(0, _database.InitializationProgress.Percentage);
        Assert.Equal(ProgressState.InProgress, _database.InitializationProgress.State);
    }

    #endregion
}
