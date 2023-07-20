using System.Collections.Generic;
using MapGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace MapGen.Map.Brushes.ObstacleSpawner
{
    
    [CreateAssetMenu(fileName = "Obstacle Brush Settings", menuName = "MapGen/Brushes/Obstacles/Brush Settings", order = 0)]
    public class ObstacleBrushSettings : ScriptableObject
    {
        [Header("Random Settings")]
        [SerializeField] private RandomSettings randomSettings;

        [Header("Object Placement Noise")]
        [SerializeField] private Noise.Noise objectPlacementNoise;
        
        [Header("Obstacles")]
        [SerializeField] private List<Placable> placables;
        [SerializeField] private float _maxObstacleRotation;
        [SerializeField] [Range(0,100)] private float objectPlacementThreshold;
        
        
        public List<Placable> Placables => placables;
        public float MaxObstacleRotation => _maxObstacleRotation;
        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise ObjectPlacementNoise => objectPlacementNoise;
        public float ObjectPlacementThreshold => objectPlacementThreshold;


    }
}