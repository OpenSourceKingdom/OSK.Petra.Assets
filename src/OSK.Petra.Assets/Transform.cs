using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets;

public abstract class Transform<TPosition, TRotation>: ITransform
    where TPosition: struct
    where TRotation: struct
{
    public TPosition Position { get; set; }

    public TRotation Rotation { get; set; }
}
