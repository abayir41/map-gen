using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.Mountains;
using MapGen.Map.Brushes.TunnelBrush;
using MapGen.Random;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.NormalMap
{
    [CreateAssetMenu(fileName = "Map Brush", menuName = "MapGen/Brushes/Normal Map/Brush", order = 0)]
    public class MapBrush : MultipleCellEditableBrush, IRandomBrush
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
        protected override int HitBrushHeight => 1;

        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new MultipleCellEditCommand(WorldCreator.Instance,this, selectedCells, grid);
        }
        
        public List<SpawnData> GenerateMap(List<Vector3Int> selectedCells, Grid grid)
        {
            SetRandomSeed();
            return Paint(selectedCells, grid);
        }

        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(RandomSettings.GetSeed());
        }

        public override List<SpawnData> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var result = new List<SpawnData>();
            
            if (MapParts.HasFlag(MapParts.Ground))
            {
                var ground = _groundBrush.Paint(selectedCells, grid);
                result.AddRange(ground);
            }

            if (MapParts.HasFlag(MapParts.Mountains))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * MOUNTAINS_LEVEL);
                var mountain = _mountainBrush.Paint(offsetedCells, grid);
                result.AddRange(mountain);
            }

            if (MapParts.HasFlag(MapParts.Tunnels))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * TUNNEL_LEVEL);
                var tunnels = _autoTunnelBrush.Paint(offsetedCells, grid);
                result.AddRange(tunnels);
            }

            if (MapParts.HasFlag(MapParts.Obstacles))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * OBSTACLES_LEVEL);
                var obstacles = _obstaclesBrush.Paint(offsetedCells, grid);
                result.AddRange(obstacles);
            }

            return result;
        }
    }
}