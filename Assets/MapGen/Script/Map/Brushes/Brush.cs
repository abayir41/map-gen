using System.Collections.Generic;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class Brush : ScriptableObject
    {
        [SerializeField] private bool _supportMultipleCells = true;
        [SerializeField] private List<BrushArea> _brushAreas;

        public bool SupportMultipleCells => _supportMultipleCells;
        public virtual List<BrushArea> BrushAreas => _brushAreas;
        
        
        public abstract string BrushName { get; }
        public abstract void Paint(List<Vector3Int> selectedCells, Grid grid);
        
    }
}