using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    public abstract class BrushArea : ScriptableObject
    {
        public abstract List<Vector3Int> GetBrushArea(Vector3Int startPoint);
        
        protected List<Vector3Int> ApplyOffsetToPoss(List<Vector3Int> poss, Vector3Int offset)
        {
            return poss.ConvertAll(input => input + offset);
        }
    }
}