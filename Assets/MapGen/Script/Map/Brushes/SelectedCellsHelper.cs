using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map.Brushes
{
    public class SelectedCellsHelper
    {
        public Grid Grid { get; }
        public List<GridCell> Cells { get; }

        public int MaxX { get; }
        public int MaxY { get; }
        public int MaxZ { get; }

        public int MinX { get; }
        public int MinY { get; }
        public int MinZ { get; }
        
        public int XWidth { get; }
        public int ZWidth { get; }

        public SelectedCellsHelper(List<GridCell> cells, Grid grid)
        {
            Grid = grid;
            Cells = cells;
            
            MaxX = Cells.Max(cell => cell.Position.x);
            MaxY = Cells.Max(cell => cell.Position.y);
            MaxZ = Cells.Max(cell => cell.Position.z);

            MinX = Cells.Min(cell => cell.Position.x);
            MinY = Cells.Min(cell => cell.Position.y);
            MinZ = Cells.Min(cell => cell.Position.z);

            XWidth = MaxX - MinX;
            ZWidth = MaxZ - MinZ;
        }
        
        public List<GridCell> GetYAxisOfGrid(int y)
        {
            return Cells.Where(cell => cell.Position.y == y).ToList();
        }

        public bool IsPosOutsideOfGrid(Vector3Int pos)
        {
            return Cells.All(cell => cell.Position != pos);
        }

        public GridCell GetCell(Vector3Int pos)
        {
            return Cells.First(cell => cell.Position == pos);
        }

        public bool IsPlacableSuitable(GridCell pos, Placable placable, float rotation)
        {
            var requiredGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Required);
            foreach (var requiredCells in requiredGrids)
            {
                foreach (var placableRequiredCell in requiredCells.CellPositions)
                {
                    var checkedGridPos = pos.Position + Grid.RotateObstacleVector(rotation, placableRequiredCell);
                    if (IsPosOutsideOfGrid(checkedGridPos) || Cells.All(cell => cell.Position != checkedGridPos))
                    {
                        return false;
                    }
                    
                    var cell = GetCell(checkedGridPos);
                    if (cell.CellState is not CellState.CanBeFilled)
                    {
                        return false;
                    }
                }
            }

            var shouldPlaceOnGroundGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.ShouldPlaceOnGround);
            foreach (var shouldPlaceOnGroundGrid in shouldPlaceOnGroundGrids)
            {
                foreach (var placableShouldPlacedOnGroundGrid in shouldPlaceOnGroundGrid.CellPositions)
                {
                    var checkedGridPos =
                        pos.Position + Grid.RotateObstacleVector(rotation, placableShouldPlacedOnGroundGrid);

                    var cell = GetCell(checkedGridPos);
                    if (cell.CellLayer != CellLayer.CanPlacableGround)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}