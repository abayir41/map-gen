using System;

namespace MapGen.Map
{
    [Flags]
    public enum MapGizmos
    {
        None = 0,
        Filled = 1 << 0,
        Locked = 1 << 1,
        CanBeFilled = 1 << 2,
        PlacableGround = 1 << 3,
        Paths = 1 << 4,
        Edges = 1 << 5,
    }
}