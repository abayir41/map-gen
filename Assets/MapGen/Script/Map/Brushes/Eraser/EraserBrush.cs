using System.Collections.Generic;
using MapGen.Map.Brushes.BrushAreas;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Eraser
{ 
    [CreateAssetMenu(fileName = "Eraser", menuName = "MapGen/Brushes/Eraser", order = 0)]
    public class EraserBrush : MultipleCellEditableBrush
    {
        public override string BrushName => "Eraser";
        
        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
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