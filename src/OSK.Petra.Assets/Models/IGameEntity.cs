namespace OSK.Petra.Assets.Models;

/// <summary>
/// An asset that is uniquely handled within the asset system
/// </summary>
/// <typeparam name="TDescriptor">The type of descriptor information the entity utilizes</typeparam>
public interface IGameEntity<TDescriptor>
    where TDescriptor: IEntityDescriptor
{
    /// <summary>
    /// The descriptor that is associated to this entity
    /// </summary>
    TDescriptor Descriptor { get; set; }
}
