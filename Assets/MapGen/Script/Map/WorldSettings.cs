using UnityEngine;

namespace MapGen.Map
{
    [CreateAssetMenu(fileName = "World Settings", menuName = "MapGen/World/Settings", order = 0)]
    public class WorldSettings : ScriptableObject
    {
        public static readonly Vector3 PLANE_HEIGHT = new Vector3(0, 0, 0);
        public static readonly Vector3 PLANE_NORMAL = Vector3.up;
        public static Vector3Int GRID_CELL_REAL_WORLD_SIZE = Vector3Int.one;
        
    }
}