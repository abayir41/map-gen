using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    [CreateAssetMenu(fileName = "Single Cell", menuName = "MapGen/Brush Areas/Single Cell", order = 0)]
    public class SingleCellBrushArea : BrushArea
    {
        public override List<Vector3Int> GetBrushArea(Vector3Int startPoint)
        {
            return new List<Vector3Int>() { startPoint };
        }
    }
}