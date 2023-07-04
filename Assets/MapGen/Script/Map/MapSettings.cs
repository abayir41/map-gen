using System;
using System.Collections.Generic;
using System.Linq;
using MapGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace MapGen.Map
{
    
    [CreateAssetMenu(fileName = "Map Settings", menuName = "MapGen/Map/Settings", order = 0)]
    public class MapSettings : ScriptableObject
    {
        ////////////
        /// Editor
        ///////////
        [SerializeField] [HideInInspector] public bool objectPlacementFoldout;
        [SerializeField] [HideInInspector] public bool groundNoiseFoldout;
        [SerializeField] [HideInInspector] public bool randomSettingsFoldout;

        
        [Header("Map Settings")]
        [SerializeField]
        private int x;
        
        [SerializeField]
        private int y;
        
        [SerializeField] 
        private int z;

        [SerializeField] 
        private int maxHeight;
        
        [Header("Map Obstacles")]
        [SerializeField] private Placable ground;
        [SerializeField] private Placable wall;
        [SerializeField] private List<Placable> placables;
        
        [Header("Random Settings")]
        [SerializeField] 
        private RandomSettings randomSettings;

        [Header("Object Placement Random")]
        [SerializeField] 
        private Noise.Noise objectPlacementNoise;
        
        [Range(0,100)]
        [SerializeField]
        private float objectPlacementThreshold;
        
        [SerializeField]
        private int iterationAmount;
        
        [Header("Ground Random")]
        [SerializeField] 
        private Noise.Noise groundPlacementNoise;

        [SerializeField] 
        private float groundHeightFactor;
        
        [SerializeField] 
        private float groundMoveDownFactor;

        public Placable Ground => ground;
        public Placable Wall => wall;
        public List<Placable> Placables => placables;
        public int MaxHeight => maxHeight;
        public float ObjectPlacementThreshold => objectPlacementThreshold;
        public int IterationAmount => iterationAmount;
        public int X => Mathf.Clamp(x, 3, 200);
        public int Y => Mathf.Clamp(y, 4, 50);
        public int Z => Mathf.Clamp(z, 3, 200);
        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise ObjectPlacementNoise => objectPlacementNoise;
        public Noise.Noise GroundPlacementNoise => groundPlacementNoise;
        public float GroundHeightFactor => groundHeightFactor;
        public float GroundMoveDownFactor => groundMoveDownFactor;
    }
}