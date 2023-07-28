using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.TunnelBrush
{
    
    [CreateAssetMenu(fileName = "Auto Tunnel Brush", menuName = "MapGen/Brushes/Tunnel/Auto Tunnel Brush", order = 0)]
    public class AutoTunnelBrush: Brush
    {
        [SerializeField] private TunnelBrushSettings _tunnelBrushSettings;
        
        public override string BrushName => "Auto Tunnel";

        private TunnelBrushHelper _helper;
        
        public override void Paint(List<Vector3Int> selectedCells, Grid grid, Vector3Int startPoint)
        {
            var layer = selectedCells.First().y;
            _helper = new TunnelBrushHelper(selectedCells, grid, _tunnelBrushSettings);
            MakeTunnels(layer, grid);
        }
        
        [MethodTimer]
        private List<Placable> MakeTunnels(int layer, Grid grid)
        {
            var edgeTunnelGridCells = _helper.FindEdgeTunnelGridCells(layer);
            var groupedEdgeTunnelGridCells = _helper.GroupEdgeGroundTunnelGrid(edgeTunnelGridCells);
            var tunnelPaths = _helper.FindAllEdgeToEdgePathsFromGroups(groupedEdgeTunnelGridCells); 
            
            if (tunnelPaths.Count == 0) return null;
            
            var heightFilteredTunnels = _helper.FilterByHeight(tunnelPaths);
            var orderedTunnels = heightFilteredTunnels.OrderByDescending(_helper.FindHeightAverageOfPath).ToList();

            var cachedPaths = new List<List<GridCell>>();
            var result = new List<Placable>();
            foreach (var path in orderedTunnels)
            {
                if (_helper.CanTunnelSpawnable(path, cachedPaths))
                {
                    var placable = CreateTunnel(path, grid);
                    cachedPaths.Add(path);
                    result.AddRange(placable);
                }
            }
            

            return result;
        }
        
        [MethodTimer]
        private List<Placable> CreateTunnel(List<GridCell> path, Grid grid)
        {
            var start = path.First();
            var end = path.Last();
            var direction = end.CellPosition - start.CellPosition;
            var direction2D = new Vector2Int(direction.x, direction.z);
            var rotationDegree = Mathf.Atan2(direction2D.y, direction2D.x) / Mathf.PI * 180;
            rotationDegree *= -1;
            var rotationDegreeInt = Mathf.RoundToInt(rotationDegree);

            var result = new List<Placable>();

            foreach (var pathPoint in path)
            {
                var tunnelDestroyGrids = _tunnelBrushSettings.TunnelBrush.Grids.Where(placableGrid =>
                    placableGrid.PlacableCellType == PlacableCellType.TunnelDestroy);
                foreach (var tunnelDestroyGrid in tunnelDestroyGrids)
                {
                    foreach (var tunnelBrushDestroyPoint in tunnelDestroyGrid.CellPositions)
                    {
                        var rotatedVector = pathPoint.CellPosition +
                                            tunnelBrushDestroyPoint.RotateVector(rotationDegreeInt, _tunnelBrushSettings.TunnelBrush.Origin);

                        if (!grid.IsCellExist(rotatedVector, out var cell)) continue;

                        if (cell.Item is null or TunnelPlacable) continue;

                        WorldCreator.Instance.DestroyItem(cell.Item);
                    }
                }
                
                if (grid.IsPlacableSuitable(pathPoint.CellPosition, _tunnelBrushSettings.TunnelBrush, rotationDegreeInt))
                {
                    var placable = WorldCreator.Instance.SpawnObject(pathPoint.CellPosition, _tunnelBrushSettings.TunnelBrush, CellLayer.Obstacle, rotationDegreeInt);
                    result.Add(placable);
                }
            }

            return result;
        }
    }
}