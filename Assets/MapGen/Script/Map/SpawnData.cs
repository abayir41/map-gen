using MapGen.GridSystem;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map
{
    public struct SpawnData
    {
        public readonly Vector3Int SpawnPos;
        public readonly Placable Prefab;
        public readonly int Rotation;
        public readonly CellLayer CellLayer;
        public readonly string ObjName;

        public SpawnData(Vector3Int spawnPos, Placable prefab, int rotation, CellLayer cellLayer, string objName = null)
        {
            SpawnPos = spawnPos;
            Prefab = prefab;
            Rotation = rotation;
            CellLayer = cellLayer;
            ObjName = objName;
        }
    }
}