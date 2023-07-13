using MapGen.Placables;
using UnityEngine;
using GridCell = MapGen.GridSystem.Obsolete.GridCell;

namespace MapGen.GridSystem.Obsolete
{
    public class GridHelper
    {
        private GridSystem.Obsolete.Grid _grid;

        public GridHelper(GridSystem.Obsolete.Grid grid)
        {
            _grid = grid;
        }
        
        public bool IsPlacableSuitable(GridCell pos, Placable placable, float rotation)
        {
            var requiredGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Required);
            foreach (var requiredCells in requiredGrids)
            {
                foreach (var placableRequiredCell in requiredCells.CellPositions)
                {
                    var checkedGridPos = pos.GlobalPosition + RotateObstacleVector(rotation, placableRequiredCell);
                    if (IsPosOutsideOfGrid(checkedGridPos))
                    {
                        return false;
                    }
                    
                    var cell = _grid.GetCell(checkedGridPos);
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
                        pos.GlobalPosition + RotateObstacleVector(rotation, placableShouldPlacedOnGroundGrid);

                    var cell = _grid.GetCell(checkedGridPos);
                    if (cell.CellLayer != CellLayer.CanPlacableGround)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public bool IsPosOutsideOfGrid(Vector3Int pos)
        {
            return pos.x >= _grid.Cells.GetLength(0) || pos.x < 0 || 
                   pos.y >= _grid.Cells.GetLength(1) || pos.y < 0 ||
                   pos.z >= _grid.Cells.GetLength(2) || pos.z < 0;
        }

        public static Vector3Int RotateObstacleVector(float angle, Vector3Int vector3Int)
        {
            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var result = rotation * vector3Int;
            var resultAsVector3Int = new Vector3Int(Mathf.RoundToInt(result.x), Mathf.RoundToInt(result.y),
                Mathf.RoundToInt(result.z));

            return resultAsVector3Int;
        }
    }
}