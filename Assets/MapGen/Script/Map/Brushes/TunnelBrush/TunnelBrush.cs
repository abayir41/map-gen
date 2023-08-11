using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using Plugins.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.TunnelBrush
{
    [CreateAssetMenu(fileName = "Tunnel Brush", menuName = "MapGen/Brushes/Tunnel/Tunnel Brush", order = 0)]
    public class TunnelBrush : MultipleCellEditableBrush
    {
        
        [Header("Tunnel")]
        [SerializeField] private TunnelPlacable _tunnelBrushPlacable;

        public override string BrushName => "Tunnel Brush";
        protected override int HitBrushHeight => 1;
        public TunnelPlacable TunnelBrushPlacable => _tunnelBrushPlacable;

        
        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new TunnelCommand(WorldCreator.Instance, this, new List<Vector3Int>(selectedCells), grid);
        }

        public TunnelPaintData Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            return MakeTunnels(selectedCells, grid);
        }
        
        [MethodTimer]
        private TunnelPaintData MakeTunnels(List<Vector3Int> path, Grid grid)
        {
            var result = new TunnelPaintData(new List<SpawnData>(), new List<SpawnData>());

            var placable = CreateTunnel(path, grid);
            result.NewSpawnedObjects.AddRange(placable.NewSpawnedObjects);
            result.DestroyedObjects.AddRange(placable.DestroyedObjects);
            
            WorldCreator.Grid.RegenerateShouldPlaceOnGrounds();

            return result;
        }
        
        [MethodTimer]
        private TunnelPaintData CreateTunnel(List<Vector3Int> path, Grid grid)
        {
            var start = path.First();
            var end = path.Last();
            var direction = end - start;
            var direction2D = new Vector2Int(direction.x, direction.z);
            var rotationDegree = Mathf.Atan2(direction2D.y, direction2D.x) / Mathf.PI * 180;
            rotationDegree *= -1;
            var rotationDegreeInt = Mathf.RoundToInt(rotationDegree);

            var result = new TunnelPaintData(new List<SpawnData>(), new List<SpawnData>());

            foreach (var pathPoint in path)
            {
                var tunnelDestroyGrids = TunnelBrushPlacable.Grids.Where(placableGrid =>
                    placableGrid.PlacableCellType == PlacableCellType.TunnelDestroy);
                foreach (var tunnelDestroyGrid in tunnelDestroyGrids)
                {
                    foreach (var tunnelBrushDestroyPoint in tunnelDestroyGrid.CellPositions)
                    {
                        var rotatedVector = pathPoint +
                                            tunnelBrushDestroyPoint.RotateVector(rotationDegreeInt, TunnelBrushPlacable.Origin);

                        if (!grid.IsCellExist(rotatedVector, out var cell)) continue;

                        if (cell.Item is null or TunnelPlacable) continue;

                        result.DestroyedObjects.Add(cell.Item.SpawnData);
                        WorldCreator.Instance.DestroyItem(cell.Item);
                    }
                }
                
                if (grid.IsPlacableSuitable(pathPoint, TunnelBrushPlacable, rotationDegreeInt))
                {
                    var data = new SpawnData(pathPoint, TunnelBrushPlacable, rotationDegreeInt,
                        CellLayer.Obstacle); 
                    WorldCreator.Instance.SpawnObject(data);
                    result.NewSpawnedObjects.Add(data);
                }
            }

            return result;
        }
        
        private List<Vector3Int> FindPath(Vector3Int startEdge, Vector3Int endEdge)
        {
            var startPos = startEdge;
            var endPos = endEdge;
            var yLayer = startPos.y;

            var newPath =
                BresenhamLineAlgorithm.DrawLine(new Vector2Int(startPos.x, startPos.z), new Vector2Int(endPos.x, endPos.z));

            var result = new List<Vector3Int>();
            foreach (var point in newPath)
            {
                var pos = new Vector3Int(point.x, yLayer, point.y);
                result.Add(pos);
            }
            return result;
        }
    }
}