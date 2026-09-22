using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets;

/// <summary>
/// A base class for all transform related objects
/// </summary>
/// <typeparam name="TPosition">The type of position the entity uses</typeparam>
/// <typeparam name="TRotation">The type of rotation the entity uses</typeparam>
public abstract class Transform<TPosition, TRotation>: ITransform
    where TPosition: struct
    where TRotation: struct
{
    /// <summary>
    /// The current position of the entity
    /// </summary>
    public TPosition Position { get; set; }

    /// <summary>
    /// The current rotation of the entity
    /// </summary>
    public TRotation Rotation { get; set; }
}
