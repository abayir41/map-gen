using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    
    [CreateAssetMenu(fileName = "Char Position Setter Settings", menuName = "MapGen/Brush Areas/Char Position Setter Area", order = 0)]
    public class CharPositionSetterArea : BrushArea
    {
        public override List<Vector3Int> GetBrushArea(Vector3Int startPoint)
        {
            return new List<Vector3Int>() { startPoint };
        }
    }
}