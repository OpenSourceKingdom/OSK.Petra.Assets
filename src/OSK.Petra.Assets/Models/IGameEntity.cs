namespace OSK.Petra.Assets.Models;

public interface IGameEntity<TDescriptor>
    where TDescriptor: IEntityDescriptor
{
    TDescriptor Descriptor { get; set; }
}
