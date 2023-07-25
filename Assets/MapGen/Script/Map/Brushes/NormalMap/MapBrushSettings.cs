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
        [SerializeField] private MapParts _mapParts;
        
        [Header("Random Settings")]
        [SerializeField] private RandomSettings randomSettings;

        public const int MOUNTAINS_LEVEL = 1;
        public const int OBSTACLES_LEVEL = 1;
        public const int TUNNEL_LEVEL = 1;
        
        public MapParts MapParts => _mapParts;
        public RandomSettings RandomSettings => randomSettings;
    }
}