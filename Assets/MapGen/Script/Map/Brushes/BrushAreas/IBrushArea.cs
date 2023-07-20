using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    public interface IBrushArea
    {
        public List<Vector3Int> GetBrushArea();
    }
}