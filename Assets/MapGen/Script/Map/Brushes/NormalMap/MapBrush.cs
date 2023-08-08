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
            var seed = randomSettings.GetSeed();
            ICommand ground = null;
            ICommand mountains = null;
            ICommand tunnels = null;
            ICommand obstacles = null;

            if (MapParts.HasFlag(MapParts.Ground))
            {
                ground = _groundBrush.GetPaintCommand(selectedCells, grid);
            }

            if (MapParts.HasFlag(MapParts.Mountains))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * MOUNTAINS_LEVEL);
                mountains = _mountainBrush.GetPaintCommand(offsetedCells, grid, seed);
            }

            if (MapParts.HasFlag(MapParts.Tunnels))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * TUNNEL_LEVEL);
                tunnels = _autoTunnelBrush.GetPaintCommand(offsetedCells, grid);
            }

            if (MapParts.HasFlag(MapParts.Obstacles))
            {
                var offsetedCells =
                    selectedCells.ConvertAll(pos => pos + Vector3Int.up * OBSTACLES_LEVEL);
                obstacles = _obstaclesBrush.GetPaintCommand(offsetedCells, grid, seed);
            }
            
            return new MapCommand(ground, mountains, tunnels, obstacles);
        }
    }
}