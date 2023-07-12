using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using UnityEngine;

namespace MapGen.Map.Brushes.NormalMap
{
    public class MapBrushCellsHelper : SelectedCellsHelper
    {
        private MapBrushSettings MapBrushSettings { get; }


        public MapBrushCellsHelper(List<GridCell> cells, Grid grid, MapBrushSettings mapBrushSettings) : base(cells, grid)
        {
            MapBrushSettings = mapBrushSettings;
        }

        public bool IsEdgeGroundYDimensionCheck(GridCell cell)
        {
            var pos = cell.Position;
            
            var offset = pos + Vector3Int.right;
            if (!IsPosOutsideOfGrid(offset))
            {
                if (GetCell(offset).CellState is CellState.CanBeFilled)
                {
                    return true;
                }
            }
            
            offset = pos + Vector3Int.left;
            if (!IsPosOutsideOfGrid(offset))
            {
                if (GetCell(offset).CellState is CellState.CanBeFilled)
                {
                    return true;
                }
            }
            
            offset = pos + Vector3Int.back;
            if (!IsPosOutsideOfGrid(offset))
            {
                if (GetCell(offset).CellState is CellState.CanBeFilled)
                {
                    return true;
                }
            }
            
            offset = pos + Vector3Int.forward;
            if (!IsPosOutsideOfGrid(offset))
            {
                if (GetCell(offset).CellState is CellState.CanBeFilled)
                {
                    return true;
                }
            }

            return false;
        }

        public List<List<GridCell>> FilterByHeight(List<List<GridCell>> paths)
        {
            var result = new List<List<GridCell>>();
            
            foreach (var path in paths)
            {
                var average = FindHeightAverageOfPath(path);
                
                if(average > MapBrushSettings.TunnelAverageMinHeight)
                    result.Add(path);
            }

            return result;
        }
        
        public int FindLengthOfPath(List<GridCell> path)
        {
            return (path.First().Position - path.Last().Position).sqrMagnitude;
        }
        
        public List<List<GridCell>> FilterPathsByLenght(List<List<GridCell>> paths)
        {
            return paths.Where(path =>
                FindLengthOfPath(path) > MapBrushSettings.TunnelMinLength).ToList();
        }
        
        public float FindHeightAverageOfPath(List<GridCell> path)
        {
            var sum = 0;
            foreach (var gridElement in path)
            {
                var cell = gridElement;
                while (cell.CellState == CellState.Filled)
                {
                    sum++;
                    
                    var pos = cell.Position;
                    pos += Vector3Int.up;
                        
                    if(IsPosOutsideOfGrid(pos))
                        break;
                        
                    cell = GetCell(new Vector3Int(pos.x, pos.y, pos.z));
                }
            }

            var average = (float) sum / path.Count;
            return average;
        }
        
        public bool CanTunnelSpawnable(List<GridCell> tunnelPath, List<List<GridCell>> otherTunnels)
        {
            foreach (var savedPath in otherTunnels)
            foreach (var pathGrid in tunnelPath)
            foreach (var savedPathGrid in savedPath)
            {
                var distance = (savedPathGrid.Position - pathGrid.Position).sqrMagnitude;
                if (distance < MapBrushSettings.BetweenTunnelMinSpace) return false;
            }

            return true;
        }
    }
}