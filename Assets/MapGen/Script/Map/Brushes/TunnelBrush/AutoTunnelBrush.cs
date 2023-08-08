using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
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
    public class AutoTunnelBrush : MultipleCellEditableBrush
    {
        [Header("Tunnel")] 
        [SerializeField] private float tunnelMinLength;
        [SerializeField] private float tunnelAverageMinHeight;
        [SerializeField] private float betweenTunnelMinSpace;
        [SerializeField] private TunnelPlacable _tunnelBrush;

        public TunnelPlacable TunnelBrush => _tunnelBrush;
        public float TunnelMinLength => tunnelMinLength;
        public float TunnelAverageMinHeight => tunnelAverageMinHeight;
        public float BetweenTunnelMinSpace => betweenTunnelMinSpace;
        public override string BrushName => "Auto Tunnel";
        protected override int HitBrushHeight => 1;

        private TunnelBrushHelper _helper;

        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new AutoTunnelCommand(WorldCreator.Instance, this, new List<Vector3Int>(selectedCells), grid);
        }

        public TunnelPaintData Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var layer = selectedCells.First().y;
            _helper = new TunnelBrushHelper(selectedCells, grid, this);
            return MakeTunnels(layer, grid);
        }
        
        [MethodTimer]
        private TunnelPaintData MakeTunnels(int layer, Grid grid)
        {
            var edgeTunnelGridCells = _helper.FindEdgeTunnelGridCells(layer);
            var groupedEdgeTunnelGridCells = _helper.GroupEdgeGroundTunnelGrid(edgeTunnelGridCells);
            var tunnelPaths = _helper.FindAllEdgeToEdgePathsFromGroups(groupedEdgeTunnelGridCells); 
            
            if (tunnelPaths.Count == 0) return new TunnelPaintData(new List<SpawnData>(), new List<SpawnData>());
            
            var heightFilteredTunnels = _helper.FilterByHeight(tunnelPaths);
            var orderedTunnels = heightFilteredTunnels.OrderByDescending(_helper.FindHeightAverageOfPath).ToList();

            var cachedPaths = new List<List<GridCell>>();
            var result = new TunnelPaintData(new List<SpawnData>(), new List<SpawnData>());
            foreach (var path in orderedTunnels)
            {
                if (_helper.CanTunnelSpawnable(path, cachedPaths))
                {
                    var placable = CreateTunnel(path, grid);
                    cachedPaths.Add(path);
                    result.NewSpawnedObjects.AddRange(placable.NewSpawnedObjects);
                    result.DestroyedObjects.AddRange(placable.DestroyedObjects);
                }
            }

            return result;
        }
        
        [MethodTimer]
        private TunnelPaintData CreateTunnel(List<GridCell> path, Grid grid)
        {
            var start = path.First();
            var end = path.Last();
            var direction = end.CellPosition - start.CellPosition;
            var direction2D = new Vector2Int(direction.x, direction.z);
            var rotationDegree = Mathf.Atan2(direction2D.y, direction2D.x) / Mathf.PI * 180;
            rotationDegree *= -1;
            var rotationDegreeInt = Mathf.RoundToInt(rotationDegree);

            var result = new TunnelPaintData(new List<SpawnData>(), new List<SpawnData>());

            foreach (var pathPoint in path)
            {
                var tunnelDestroyGrids = TunnelBrush.Grids.Where(placableGrid =>
                    placableGrid.PlacableCellType == PlacableCellType.TunnelDestroy);
                foreach (var tunnelDestroyGrid in tunnelDestroyGrids)
                {
                    foreach (var tunnelBrushDestroyPoint in tunnelDestroyGrid.CellPositions)
                    {
                        var rotatedVector = pathPoint.CellPosition +
                                            tunnelBrushDestroyPoint.RotateVector(rotationDegreeInt, TunnelBrush.Origin);

                        if (!grid.IsCellExist(rotatedVector, out var cell)) continue;

                        if (cell.Item is null or TunnelPlacable) continue;

                        result.DestroyedObjects.Add(cell.Item.SpawnData);
                        WorldCreator.Instance.DestroyItem(cell.Item);
                    }
                }
                
                if (grid.IsPlacableSuitable(pathPoint.CellPosition, TunnelBrush, rotationDegreeInt))
                {
                    var data = new SpawnData(pathPoint.CellPosition, TunnelBrush, rotationDegreeInt,
                        CellLayer.Obstacle); 
                    WorldCreator.Instance.SpawnObject(data);
                    result.NewSpawnedObjects.Add(data);
                }
            }

            return result;
        }
        
    }
}