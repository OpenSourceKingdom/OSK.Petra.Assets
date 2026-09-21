using System;

namespace OSK.Petra.Assets.Models;

public readonly struct AssetTemplateReference<TTemplate, TTransform>
    : IEntityAssetReference<TTransform>
    where TTransform: ITransform
{
    #region Variables

    public EntityAssetIdentifier? AssetIdentifier { get; }

    public TTemplate Template { get; }

    public bool CleanupOnDispose { get; }

    #endregion

    #region Constructors

    public AssetTemplateReference()
        : this(null, default, false)
    {
    }

    public AssetTemplateReference(TTemplate template, bool cleanupOnDispose = false)
        : this(null, template, cleanupOnDispose)
    {
    }

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
