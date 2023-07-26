using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MapGen.Map
{
    public class RotationMap
    {
        public Dictionary<Vector2Int,RotationMapCell> RotationGrid { get; private set; }
        
        public RotationMap()
        {
            RotationGrid = new Dictionary<Vector2Int, RotationMapCell>();
        }

        public void SetCell(Vector2Int pos, RotationMapCell cell)
        {
            RotationGrid.Add(pos, cell);
        }

        public bool IsCellExist(Vector2Int pos, out RotationMapCell cell)
        {
            return RotationGrid.TryGetValue(pos, out cell);
        }
    }
}