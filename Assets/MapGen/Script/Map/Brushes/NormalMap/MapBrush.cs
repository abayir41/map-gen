using System.Collections.Generic;
using MapGen.Map.Brushes.Mountains;
using MapGen.Map.Brushes.TunnelBrush;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.NormalMap
{
    [CreateAssetMenu(fileName = "Map Brush", menuName = "MapGen/Brushes/Normal Map/Brush", order = 0)]
    public class MapBrush : Brush
    {
        [SerializeField] private MapBrushSettings _mapBrushSettings;
        [SerializeField] private GroundBrush.GroundBrush _groundBrush;
        [SerializeField] private ObstacleSpawner.ObstaclesBrush _obstaclesBrush;
        [SerializeField] private MountainBrush _mountainBrush;
        [SerializeField] private AutoTunnelBrush _autoTunnelBrush;


        public override string BrushName => "Map";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid, Vector3Int startPoint)
        {
            GenerateMap(selectedCells, startPoint ,grid);
        }
        
        [MethodTimer]
        public void GenerateMap(List<Vector3Int> selectedCells, Vector3Int startPoint, Grid grid)
        {
            SetRandomSeed();
            Generate(selectedCells, startPoint, grid);
        }

        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(_mapBrushSettings.RandomSettings.GetSeed());
        }

        private void Generate(List<Vector3Int> selectedCells, Vector3Int startPoint, Grid grid)
        {
            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Ground))
            {
                _groundBrush.Paint(selectedCells, grid, startPoint);
            }

            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Mountains))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * MapBrushSettings.MOUNTAINS_LEVEL);
                _mountainBrush.Paint(offsetedCells, grid, startPoint);
            }

            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Tunnels))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * MapBrushSettings.TUNNEL_LEVEL);
                _autoTunnelBrush.Paint(offsetedCells, grid, startPoint);
            }

            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Obstacles))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * MapBrushSettings.OBSTACLES_LEVEL);
                _obstaclesBrush.Paint(offsetedCells, grid, startPoint);
            }
        }
    }
}