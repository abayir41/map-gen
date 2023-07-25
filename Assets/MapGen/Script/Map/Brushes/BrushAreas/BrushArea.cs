using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    public abstract class BrushArea : ScriptableObject
    {
        public abstract List<Vector3Int> GetBrushArea();
    }
}