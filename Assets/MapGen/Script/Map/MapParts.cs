using System;

namespace MapGen.Map
{
    [Flags]
    public enum MapParts
    {
        None = 0,
        Ground = 1 << 0,
        Mountains = 1 << 1, 
        Tunnels = 1 << 2,
        Obstacles = 1 << 3,
        Walls = 1 << 4,
    }
}