using System.Collections.Generic;
using MapGen.GridSystem;
using UnityEngine;

namespace MapGen.Map.MapEdit.Brushes
{
    [CreateAssetMenu(fileName = "Selectable Map Brush", menuName = "MapGen/Selectable Grids/Map Brush", order = 0)]
    public class MapBrushSettings : BrushSettings
    {
        public override Vector2Int BrushSizeX => _brushSizeX;
        public override Vector2Int BrushSizeY => _brushSizeY;
        public override Vector2Int BrushSizeZ => _brushSizeZ;

        public override List<GridCell> GetWantedAreaAsGridCell(List<GridCell> selectedCells, Grid grid)
        {
            var maxY = grid.Cells.GetLength(1);
            var result = new List<GridCell>();
            foreach (var selectedCell in selectedCells)
            {
                for (var y = 0; y < maxY; y++)
                {
                    var targetCell = grid.GetSameYAxisCell(selectedCell, y);
                    if (result.Contains(targetCell)) continue;
                    result.Add(targetCell);
                }
            }

            return result;
        }

        public override List<Vector3Int> GetWantedAreaAsV3Int(List<GridCell> selectedCells, Grid grid)
        {
            var cells = GetWantedAreaAsGridCell(selectedCells, grid);
            return cells.ConvertAll(input => input.Position);
        }
    }
}