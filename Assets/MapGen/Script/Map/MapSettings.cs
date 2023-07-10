using System;
using System.Collections.Generic;
using MapGen.Placables;
using MapGen.Random;
using MapGen.TunnelSystem;
using UnityEditor.Performance.ProfileAnalyzer;
using UnityEngine;

namespace MapGen.Map
{
    
    [CreateAssetMenu(fileName = "Map Settings", menuName = "MapGen/Map/Settings", order = 0)]
    public class MapSettings : ScriptableObject
    {
        private const int MAX_SIZE = 200;
        private const int MIN_SIZE = 5;
        
        [Header("Map Settings")]
        [SerializeField] private Vector3Int _mapSize;
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
        
        [Header("Ground Random")]
        [SerializeField] private Noise.Noise groundPlacementNoise;
        [SerializeField] private float groundHeightFactor;
        [SerializeField] private float groundMoveDownFactor;

        [Header("Tunnel")] 
        [SerializeField] private int tunnelYLevel;
        [SerializeField] private float tunnelMinLength;
        [SerializeField] private float tunnelAverageMinHeight;
        [SerializeField] private float betweenTunnelMinSpace;
        [SerializeField] private TunnelBrush _tunnelBrush;


        public Vector3Int MapSize => _mapSize;
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
        

        private void OnValidate()
        {
            if (_mapSize.x > MAX_SIZE)
            {
                Debug.LogWarning($"X value exceed Max({MAX_SIZE}), setting to -> {MAX_SIZE}");
            }
            else if (_mapSize.x < MIN_SIZE)
            {
                Debug.LogWarning($"X value exceed Min({MIN_SIZE}), setting to -> {MIN_SIZE}");
            }
            
            if (_mapSize.y > MAX_SIZE)
            {
                Debug.LogWarning($"Y value exceed Max({MAX_SIZE}), setting to -> {MAX_SIZE}");
            }
            else if (_mapSize.y < 5)
            {
                Debug.LogWarning($"Y value exceed Min({MIN_SIZE}), setting to -> {MIN_SIZE}");
            }
            
            if (_mapSize.z > MAX_SIZE)
            {
                Debug.LogWarning($"Z value exceed Max({MAX_SIZE}), setting to -> {MAX_SIZE}");
            }
            else if (_mapSize.z < 5)
            {
                Debug.LogWarning($"Z value exceed Min({MIN_SIZE}), setting to -> {MIN_SIZE}");
            }

            var x = Mathf.Clamp(_mapSize.x, MIN_SIZE, MAX_SIZE);
            var y = Mathf.Clamp(_mapSize.y, MIN_SIZE, MAX_SIZE);
            var z = Mathf.Clamp(_mapSize.z, MIN_SIZE, MAX_SIZE);
            _mapSize = new Vector3Int(x, y, z);
        }
    }
}