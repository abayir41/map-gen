using System.Collections.Generic;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class Brush : ScriptableObject
    {
        [SerializeField] private List<BrushArea> _brushAreas;
        public virtual List<BrushArea> BrushAreas => _brushAreas;
        
        
        public abstract string BrushName { get; }
        public abstract void Paint(List<Vector3Int> selectedCells, Grid grid);
        
    }
}