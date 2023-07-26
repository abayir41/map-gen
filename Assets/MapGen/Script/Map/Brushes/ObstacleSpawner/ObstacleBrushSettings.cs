using System.Collections.Generic;
using MapGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace MapGen.Map.Brushes.ObstacleSpawner
{
    [CreateAssetMenu(fileName = "Obstacle Brush Settings", menuName = "MapGen/Brushes/Obstacles/Brush Settings", order = 0)]
    public class ObstacleBrushSettings : ScriptableObject
    {
        [Header("Object Placement Probability Settings")]
        [SerializeField] private Noise.Noise objectPlacementNoise;
        [SerializeField] private RandomSettings randomSettings;
        [SerializeField] private bool _useNoiseMap;
        public float[,] ObjectPlacementProbability { get; set; }
        public RotationMap RotationMap { get; set; }
        
        [Header("Obstacles")]
        [SerializeField] private List<Placable> placables;
        [SerializeField] private float _maxObstacleRotation;
        [SerializeField] [Range(0,100)] private float objectPlacementThreshold;
        
        
        public List<Placable> Placables => placables;
        public float MaxObstacleRotation => _maxObstacleRotation;
        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise ObjectPlacementNoise => objectPlacementNoise;
        public float ObjectPlacementThreshold => objectPlacementThreshold;

        public bool UseNoiseMap
        {
            get => _useNoiseMap;
            set => _useNoiseMap = value;
        }
    }
}