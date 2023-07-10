using MapGen.Placables;
using UnityEngine;

namespace MapGen.TunnelSystem
{
    public class TunnelBrush : Placable
    {
        [SerializeField] protected PlacableGrid _destroyPoints;
        public PlacableGrid DestroyPoints => _destroyPoints;
    }
}