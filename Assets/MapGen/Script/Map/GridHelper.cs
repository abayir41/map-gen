using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map
{
    public class GridHelper
    {
        private readonly GridCell[,,] _grids;
        private readonly MapSettings _mapSettings;

        public GridHelper(GridCell[,,] grid, MapSettings mapSettings)
        {
            _grids = grid;
            _mapSettings = mapSettings;
        }
        
        public List<List<GridCell>> FilterPathsByLenght(List<List<GridCell>> paths)
        {
            return paths.Where(path =>
                FindLengthOfPath(path) > _mapSettings.TunnelMinLength).ToList();
        }
        
        public List<List<GridCell>> FilterByHeight(List<List<GridCell>> paths)
        {
            var result = new List<List<GridCell>>();
            
            foreach (var path in paths)
            {
                var average = FindHeightAverageOfPath(path);
                
                if(average > _mapSettings.TunnelAverageMinHeight)
                    result.Add(path);
            }

            return result;
        }

        public float FindHeightAverageOfPath(List<GridCell> path)
        {
            var sum = 0;
            foreach (var gridElement in path)
            {
                var grid = gridElement;
                while (grid.CellState == CellState.Filled)
                {
                    sum++;
                    
                    var pos = grid.Position;
                    pos += Vector3Int.up;
                        
                    if(IsPosOutsideOfGrid(pos))
                        break;
                        
                    grid = _grids[pos.x, pos.y, pos.z];
                }
            }

            var average = (float) sum / path.Count;
            return average;
        }

        public int FindLengthOfPath(List<GridCell> path)
        {
            return (path.First().Position - path.Last().Position).sqrMagnitude;
        }  
        
        public bool IsPlacableSuitable(GridCell pos, Placable placable, float rotation)
        {
            var requiredGrids = placable.Grids.FindAll(grid => grid.GetCellType() == PlacableCellType.Required);
            foreach (var requiredGrid in requiredGrids)
            {
                foreach (var placableRequiredGrid in requiredGrid.CellPositions)
                {
                    var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableRequiredGrid);
                    if (IsPosOutsideOfGrid(checkedGridPos))
                    {
                        return false;
                    }

                    var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                    if (grid.CellState is not CellState.CanBeFilled)
                    {
                        return false;
                    }
                }
            }

            var shouldPlaceOnGroundGrids = placable.Grids.FindAll(grid => grid.GetCellType() == PlacableCellType.ShouldPlaceOnGround);
            foreach (var shouldPlaceOnGroundGrid in shouldPlaceOnGroundGrids)
            {
                foreach (var placableShouldPlacedOnGroundGrid in shouldPlaceOnGroundGrid.CellPositions)
                {
                    var checkedGridPos =
                        pos.Position + RotateObstacleVector(rotation, placableShouldPlacedOnGroundGrid);

                    var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                    if (grid.CellLayer != CellLayer.CanPlacableGround)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public bool IsPosOutsideOfGrid(Vector3Int pos)
        {
            return pos.x >= _grids.GetLength(0) || pos.x < 0 || 
                   pos.y >= _grids.GetLength(1) || pos.y < 0 ||
                   pos.z >= _grids.GetLength(2) || pos.z < 0;
        }
        
        public bool IsEdgeGroundYDimensionCheck(GridCell cell)
        {
            var pos = cell.Position;
            
            var offset = pos + Vector3Int.right;
            var neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrid(offset) && neighborGrid.CellState is CellState.CanBeFilled)
                return true;
            
            offset = pos + Vector3Int.left;
            neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrid(offset) && neighborGrid.CellState is CellState.CanBeFilled)
                return true;
            
            offset = pos + Vector3Int.forward;
            neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrid(offset) && neighborGrid.CellState is CellState.CanBeFilled)
                return true;
            
            offset = pos + Vector3Int.back;
            neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrid(offset) && neighborGrid.CellState is CellState.CanBeFilled)
                return true;

            return false;
        }
        
        public Vector3Int RotateObstacleVector(float angle, Vector3Int vector3Int)
        {
            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var result = rotation * vector3Int;
            var resultAsVector3Int = new Vector3Int(Mathf.RoundToInt(result.x), Mathf.RoundToInt(result.y),
                Mathf.RoundToInt(result.z));

            return resultAsVector3Int;
        }

        public bool CanTunnelSpawnable(List<GridCell> tunnelPath, List<List<GridCell>> otherTunnels)
        {
            foreach (var savedPath in otherTunnels)
            foreach (var pathGrid in tunnelPath)
            foreach (var savedPathGrid in savedPath)
            {
                var distance = (savedPathGrid.Position - pathGrid.Position).sqrMagnitude;
                if (distance < _mapSettings.BetweenTunnelMinSpace) return false;
            }

            return true;
        }
    }
}