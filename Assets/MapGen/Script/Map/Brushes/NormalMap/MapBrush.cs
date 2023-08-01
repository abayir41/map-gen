using System.Collections.Generic;
using MapGen.Map.Brushes.Mountains;
using MapGen.Map.Brushes.TunnelBrush;
using MapGen.Random;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.NormalMap
{
    [CreateAssetMenu(fileName = "Map Brush", menuName = "MapGen/Brushes/Normal Map/Brush", order = 0)]
    public class MapBrush : MultipleCellEditableBrush
    {
        [SerializeField] private GroundBrush.GroundBrush _groundBrush;
        [SerializeField] private ObstacleSpawner.ObstaclesBrush _obstaclesBrush;
        [SerializeField] private MountainBrush _mountainBrush;
        [SerializeField] private AutoTunnelBrush _autoTunnelBrush;
        [Header("Map Settings")]
        [SerializeField] private MapParts _mapParts;
        
        [Header("Random Settings")]
        [SerializeField] private RandomSettings randomSettings;

        public const int MOUNTAINS_LEVEL = 1;
        public const int OBSTACLES_LEVEL = 1;
        public const int TUNNEL_LEVEL = 1;
        
        public MapParts MapParts => _mapParts;
        public RandomSettings RandomSettings => randomSettings;

        public override string BrushName => "Map";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            GenerateMap(selectedCells ,grid);
        }
        
        [MethodTimer]
        public void GenerateMap(List<Vector3Int> selectedCells, Grid grid)
        {
            SetRandomSeed();
            Generate(selectedCells, grid);
        }

        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(RandomSettings.GetSeed());
        }

        private void Generate(List<Vector3Int> selectedCells, Grid grid)
        {
            if (MapParts.HasFlag(MapParts.Ground))
            {
                _groundBrush.Paint(selectedCells, grid);
            }

            if (MapParts.HasFlag(MapParts.Mountains))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * MOUNTAINS_LEVEL);
                _mountainBrush.Paint(offsetedCells, grid);
            }

            if (MapParts.HasFlag(MapParts.Tunnels))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * TUNNEL_LEVEL);
                _autoTunnelBrush.Paint(offsetedCells, grid);
            }

            if (MapParts.HasFlag(MapParts.Obstacles))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * OBSTACLES_LEVEL);
                _obstaclesBrush.Paint(offsetedCells, grid);
            }
        }
    }
}