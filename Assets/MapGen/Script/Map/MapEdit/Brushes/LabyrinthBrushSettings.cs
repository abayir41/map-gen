using System.Collections.Generic;
using MapGen.GridSystem;
using UnityEngine;

namespace MapGen.Map.MapEdit.Brushes
{
    
    [CreateAssetMenu(fileName = "Selectable Map Brush", menuName = "MapGen/Selectable Grids/Labyrinth Brush", order = 0)]
    public class LabyrinthBrushSettings : BrushSettings
    {
        public override Vector2Int BrushSizeX => _brushSizeX;
        public override Vector2Int BrushSizeY => _brushSizeY;
        public override Vector2Int BrushSizeZ => _brushSizeZ;

        public override List<GridCell> GetWantedAreaAsGridCell(List<GridCell> selectedCells, Grid grid)
        {
            var result = new List<GridCell>();
            foreach (var selectedCell in selectedCells)
            {
                result.AddRange(grid.GetYAxis(selectedCell));
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