using System.Collections.Generic;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Eraser
{
    
    [CreateAssetMenu(fileName = "Eraser Brush", menuName = "MapGen/Brushes/Eraser", order = 0)]
    public class EraserBrush : Brush
    {
        public override List<Placable> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            foreach (var selectedCell in selectedCells)
            {
                if (grid.IsCellExist(selectedCell, out var cell) && cell.Item != null)
                {
                    WorldCreator.Instance.DestroyItem(cell.Item);
                }
            }

            return new List<Placable>();
        }
    }
}