using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapGen.GridSystem;
using Plugins.Utilities;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.NormalMap
{
    public class MapBrushHelper : SelectedCellsHelper
    {
        private readonly Grid _grid;
        private readonly MapBrushSettings _mapBrushSettings;

        public MapBrushHelper(List<Vector3Int> cells, Grid grid, MapBrushSettings mapBrushSettings) : base(cells, grid)
        {
            _grid = grid;
            _mapBrushSettings = mapBrushSettings;
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

        [MethodTimer]
        public List<GridCell> FindEdgeTunnelGridCells(int layer)
        {
            var gridCells = new List<GridCell>();

            var yFilteredSelectedPoss = GetYAxisOfGrid(layer);
            foreach (var selectedPos in yFilteredSelectedPoss)
            {
                if(!_grid.IsCellExist(selectedPos, out var cell)) continue;
                
                if (cell.CellState == CellState.Filled && IsEdgeGroundYDimensionCheck(selectedPos))
                {
                    gridCells.Add(cell);
                }
            }
            return gridCells;
        }
        
        [MethodTimer]
        public List<List<GridCell>> GroupEdgeGroundTunnelGrid(List<GridCell> edgeNodes)
        {
            var result = new List<List<GridCell>>();
            var remainingNodes = new List<GridCell>(edgeNodes);
            
            foreach (var node in edgeNodes)
            {
                var processNode = true;
                foreach (var list in result)
                {
                    if (list.Contains(node))
                    {
                        processNode = false;
                        break;
                    }
                }

                if (!processNode) continue;

                var group = new List<GridCell> {node};
                FindAllNeighborEdges(node, remainingNodes, group);
                remainingNodes = remainingNodes.Except(group).ToList();
                result.Add(group);
            }

            return result;
        }
        
        private void FindAllNeighborEdges(GridCell start, List<GridCell> cells, List<GridCell> result)
        {
            for (var xAxis = -1; xAxis <= 1; xAxis++)
            {
                for (var yAxis = -1; yAxis <= 1; yAxis++)
                {
                    var possibleNeighbor = cells.FirstOrDefault(node =>
                        node.CellPosition == start.CellPosition + new Vector3Int(xAxis, 0, yAxis));

                    if (possibleNeighbor == null || result.Contains(possibleNeighbor)) continue;
                        
                    result.Add(possibleNeighbor);
                    FindAllNeighborEdges(possibleNeighbor, cells, result);
                }
            }
        }
        
        [MethodTimer]
        public List<List<GridCell>> FindAllEdgeToEdgePathsFromGroups(List<List<GridCell>> edgeGroups)
        {
            var paths = new List<List<GridCell>>();

            foreach (var edgeGroup in edgeGroups)
            {
                for (var i = 0; i < edgeGroup.Count - 1; i++)
                {
                    var edgeNodeA = edgeGroup[i];

                    for (var a = i + 1; a < edgeGroup.Count; a++)
                    {
                        var edgeNodeB = edgeGroup[a];
                        var distance = (edgeNodeA.CellPosition - edgeNodeB.CellPosition).sqrMagnitude;
                        if (distance < _mapBrushSettings.TunnelMinLength) continue;

                        var path = FindPath(edgeNodeA, edgeNodeB);
                        
                        paths.Add(path);
                    }
                }
            }
            return paths;
        }
        
        private List<GridCell> FindPath(GridCell startEdge, GridCell endEdge)
        {
            var startPos = startEdge.CellPosition;
            var endPos = endEdge.CellPosition;
            var yLayer = startPos.y;

            var newPath =
                BresenhamLineAlgorithm.DrawLine(new Vector2Int(startPos.x, startPos.z), new Vector2Int(endPos.x, endPos.z));

            var result = new List<GridCell>();
            foreach (var point in newPath)
            {
                var pos = new Vector3Int(point.x, yLayer, point.y);
                if (!_grid.IsCellExist(pos, out var cell))
                {
                    cell = _grid.CreateCell(pos);
                }
                
                result.Add(cell);
            }
            return result;
        }
        
        public List<List<GridCell>> FilterByHeight(List<List<GridCell>> paths)
        {
            var result = new List<List<GridCell>>();
            
            foreach (var path in paths)
            {
                var average = FindHeightAverageOfPath(path);
                
                if(average > _mapBrushSettings.TunnelAverageMinHeight)
                    result.Add(path);
            }

            return result;
        }
        
        public int FindLengthOfPath(List<GridCell> path)
        {
            return (path.First().CellPosition - path.Last().CellPosition).sqrMagnitude;
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
                if (distance < _mapBrushSettings.BetweenTunnelMinSpace) return false;
            }

            return true;
        }
    }
}