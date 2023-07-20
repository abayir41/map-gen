using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    
    [CreateAssetMenu(fileName = "Char Position Setter Settings", menuName = "MapGen/Brush Areas/Char Position Setter Area", order = 0)]
    public class CharPositionSetterArea : ScriptableObject, IBrushArea
    {
        public List<Vector3Int> GetBrushArea()
        {
            return new List<Vector3Int>() { new(0, 0, 0) };
        }
    }
}