using System.Collections.Generic;
using MapGen.Placables;
using MapGen.Random;
using MapGen.TunnelSystem;
using UnityEngine;

namespace MapGen.Map.Brushes.NormalMap
{
    
    [CreateAssetMenu(fileName = "Map Brush Settings", menuName = "MapGen/Brushes/Normal Map/Map Brush Settings", order = 0)]
    public class MapBrushSettings : ScriptableObject
    {
        [Header("Map Settings")]
        [SerializeField] private int _obstaclesMaxHeight;
        [SerializeField] private MapParts _mapParts;
        
        [Header("Map Obstacles")]
        [SerializeField] private Placable ground;
        [SerializeField] private Placable wall;
        [SerializeField] private List<Placable> placables;
        [SerializeField] private float _maxObstacleRotation;
        
        [Header("Random Settings")]
        [SerializeField] private RandomSettings randomSettings;

        [Header("Object Placement Random")]
        [SerializeField] private Noise.Noise objectPlacementNoise;
        
        [Range(0,100)]
        [SerializeField] private float objectPlacementThreshold;
        [SerializeField] private int iterationAmount;

        [Header("Mountain")] 
        public const int MOUNTAIN_Y_START_LEVEL = 1;
        
        [Header("Ground")]
        [SerializeField] private Noise.Noise groundPlacementNoise;
        [SerializeField] private float groundHeightFactor;
        [SerializeField] private float groundMoveDownFactor;
        public const int GROUND_ROTATION = 0;
        public const int GROUND_Y_LEVEL = 0;
        
        [Header("Tunnel")] 
        [SerializeField] private int tunnelYLevel;
        [SerializeField] private float tunnelMinLength;
        [SerializeField] private float tunnelAverageMinHeight;
        [SerializeField] private float betweenTunnelMinSpace;
        [SerializeField] private TunnelBrush _tunnelBrush;
        
        public int ObstaclesMaxHeight => _obstaclesMaxHeight;
        public MapParts MapParts => _mapParts;

        public Placable Ground => ground;
        public Placable Wall => wall;
        public List<Placable> Placables => placables;
        public float MaxObstacleRotation => _maxObstacleRotation;

        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise ObjectPlacementNoise => objectPlacementNoise;
        public float ObjectPlacementThreshold => objectPlacementThreshold;
        public int IterationAmount => iterationAmount;
       
        public Noise.Noise GroundPlacementNoise => groundPlacementNoise;
        public float GroundHeightFactor => groundHeightFactor;
        public float GroundMoveDownFactor => groundMoveDownFactor;
        
        public int TunnelYLevel => tunnelYLevel;
        public float TunnelMinLength => tunnelMinLength;
        public float TunnelAverageMinHeight => tunnelAverageMinHeight;
        public float BetweenTunnelMinSpace => betweenTunnelMinSpace;
        public TunnelBrush TunnelBrush => _tunnelBrush;
    }
}