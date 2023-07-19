using System.Collections.Generic;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public interface IBrush
    {
        public string BrushName { get; }
        public void Paint(List<Vector3Int> selectedCells, Grid grid);
    }
}