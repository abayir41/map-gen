using System.Collections.Generic;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Eraser
{ 
    public class EraserBrush : IBrush
    {
        public string BrushName => "Eraser";

        public void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            foreach (var selectedCell in selectedCells)
            {
                if (grid.IsCellExist(selectedCell, out var cell) && cell.Item != null)
                {
                    WorldCreator.Instance.DestroyItem(cell.Item);
                }
            }
        }
    }
}