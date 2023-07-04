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

        [SerializeField] 
        private Noise.Noise noise;
        
        [Range(0,100)][SerializeField]
        private float threshold;

        [SerializeField]
        private int iterationAmount;
        
        [SerializeField]
        protected float noiseScale;
        
        [SerializeField]
        protected Vector2 noiseOffset;


        public Placable Ground => ground;
        public Placable Wall => wall;
        public List<Placable> Placables => placables;
        public int MaxHeight => maxHeight;
        public float Threshold => threshold;
        public int IterationAmount => iterationAmount;
        public int X => Mathf.Clamp(x, 3, 200);
        public int Y => Mathf.Clamp(y, 4, 50);
        public int Z => Mathf.Clamp(z, 3, 200);
        public RandomSettings RandomSettings => randomSettings;

        private int _cachedHeight;
        private int _cachedX;
        private int _cachedY;
        private int _cachedZ;
        private float _cachedThreshold;
        private int _cachedIterationAmount;
        private float _cachedPerlinScale;
        private Vector2 _cachedPerlinOffsetScale;
        private Placable _cachedGround;
        private Placable _cachedWall;
        private List<Placable> _cachedPlacables;

        private void Awake()
        {
            _cachedPlacables = placables;
            _cachedGround = ground;
            _cachedWall = wall;
        }

        public float[,] GetNoise()
        {
            noise.noiseScale = noiseScale;
            noise.noiseOffset = noiseOffset;

            return noise.Generate(X, Z);
        }

        public bool IsAnyThingChanged()
        {
            var result = false;

            if (randomSettings is CustomRandomSettings customRandomSettings)
                result = customRandomSettings.IsThereAnyChange();


            if (_cachedX != x ||
                _cachedY != y ||
                _cachedZ != z ||
                Math.Abs(_cachedThreshold - threshold) > 0.0001f ||
                _cachedIterationAmount != iterationAmount ||
                Math.Abs(_cachedPerlinScale - noiseScale) > 0.0001f ||
                _cachedPerlinOffsetScale != noiseOffset ||
                _cachedGround != ground ||
                _cachedWall != wall ||
                _cachedPlacables.Count != placables.Count || 
                _cachedHeight != maxHeight)
            {
                result = true;
            }

            if (_cachedPlacables.Count == placables.Count)
            {
                if (_cachedPlacables.Where((t, i) => t != placables[i]).Any())
                {
                    result = true;
                }
            }

            _cachedHeight = maxHeight;
            _cachedGround = ground;
            _cachedWall = wall;
            _cachedPlacables = new List<Placable>(placables);
            _cachedX = x;
            _cachedY = y;
            _cachedZ = z;
            _cachedThreshold = threshold;
            _cachedIterationAmount = iterationAmount;
            _cachedPerlinScale = noiseScale;
            _cachedPerlinOffsetScale = noiseOffset;

            return result;
        }
    }
}