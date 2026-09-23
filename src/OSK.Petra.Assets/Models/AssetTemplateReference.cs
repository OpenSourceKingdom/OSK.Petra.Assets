using System;

namespace OSK.Petra.Assets.Models;

public readonly struct AssetTemplateReference<TTemplate, TTransform>
    : IEntityAssetReference<TTransform>
    where TTransform: ITransform
{
    #region Variables

    /// <summary>
    /// The identifier that is associated with the asset that is used for caching, descriptor, and similar handling
    /// </summary>
    public EntityAssetIdentifier? AssetIdentifier { get; }

    /// <summary>
    /// The template object that is used to create copies
    /// </summary>
    public TTemplate Template { get; }

    /// <summary>
    /// Whether the data and other logic underlying the provided <see cref="Template"/> should be disposed of when finished being used or if the application will handle this. i.e. if you're using a prefab, scene, or similar data 
    /// in your application, you likely do not want to dispose of the underlying data objects unless you know they will no longer be used.
    /// </summary>
    public bool CleanupOnDispose { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Do not use the empty constructor. Template references require a valid template to be usable.
    /// </summary>
    public AssetTemplateReference()
        : this(null, default, false)
    {
    }

    /// <summary>
    /// Create a tepmlate asset reference with a template
    /// </summary>
    /// <param name="template">The template that the reference will use</param>
    /// <param name="cleanupOnDispose">Whether the underlying template resources should be disposed when this reference is no longer needed</param>
    public AssetTemplateReference(TTemplate template, bool cleanupOnDispose = false)
        : this(null, template, cleanupOnDispose)
    {
    }

    /// <summary>
    /// Create a tepmlate asset reference with a template and an identifier
    /// </summary>
    /// <param name="template">The template that the reference will use</param>
    /// <param name="identifier">The asset identifier associated with the template</param>
    /// <param name="cleanupOnDispose">Whether the underlying template resources should be disposed when this reference is no longer needed</param>
    public AssetTemplateReference(TTemplate template, EntityAssetIdentifier identifier, bool cleanupOnDispose = false)
        : this(identifier, template, cleanupOnDispose)
    {
    }

    private AssetTemplateReference(EntityAssetIdentifier? identifier, TTemplate? template, bool cleanupOnDispose)
    {
        Template = template ?? throw new ArgumentNullException(nameof(template), "An asset reference for a template can not have an undefined template.");
        AssetIdentifier = identifier;
        CleanupOnDispose = cleanupOnDispose;
    } 

    #endregion
}
