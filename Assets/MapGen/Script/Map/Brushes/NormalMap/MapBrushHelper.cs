using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.NormalMap
{
    public class MapBrushHelper : SelectedCellsHelper
    {
        private MapBrushSettings MapBrushSettings { get; }

        public MapBrushHelper(List<Vector3Int> cells, Grid grid, MapBrushSettings mapBrushSettings) : base(cells, grid)
        {
            MapBrushSettings = mapBrushSettings;
        }

        public bool IsEdgeGroundYDimensionCheck(Vector3Int pos)
        {
            var offset = pos + Vector3Int.right;
            if (!Grid.IsCellExist(offset, out var cell) || cell.CellState is CellState.CanBeFilled)
            {
                return true;
            }
            
            offset = pos + Vector3Int.left;
            if (!Grid.IsCellExist(offset, out cell) || cell.CellState is CellState.CanBeFilled)
            {
                return true;
            }
            
            offset = pos + Vector3Int.back;
            if (!Grid.IsCellExist(offset, out cell) || cell.CellState is CellState.CanBeFilled)
            {
                return true;
            }
            
            offset = pos + Vector3Int.forward;
            if (!Grid.IsCellExist(offset, out cell) || cell.CellState is CellState.CanBeFilled)
            {
                return true;
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
            return (path.First().CellPosition - path.Last().CellPosition).sqrMagnitude;
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
                    
                    var pos = cell.CellPosition;
                    pos += Vector3Int.up;

                    if (!Grid.IsCellExist(pos, out cell))
                    {
                        break;
                    }
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
                var distance = (savedPathGrid.CellPosition - pathGrid.CellPosition).sqrMagnitude;
                if (distance < MapBrushSettings.BetweenTunnelMinSpace) return false;
            }

            return true;
        }
    }
}