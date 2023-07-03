using System;
using MapGen.Random;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.Map
{
    
    [CreateAssetMenu(fileName = "Random Settings", menuName = "MapGen/Map/Settings", order = 0)]
    public class MapSettings : ScriptableObject
    {
        [SerializeField]
        private int x;
        
        [SerializeField]
        private int y;
        
        [SerializeField] 
        private int z;
        
        [Header("Random Settings")]
        [SerializeField] 
        private RandomSettings randomSettings;
        
        [SerializeField]
        private float perlinScale;
        
        [SerializeField]
        private float perlinOffsetScale;

        [Range(0,100)][SerializeField]
        private float threshold;

        [SerializeField]
        private int iterationAmount;
        
        public float PerlinScale => perlinScale;
        public float PerlinOffsetScale => perlinOffsetScale;
        public float Threshold => threshold;
        public int IterationAmount => iterationAmount;
        
        public int X => Mathf.Clamp(x, 3, int.MaxValue);
        public int Y => Mathf.Clamp(y, 4, int.MaxValue);
        public int Z => Mathf.Clamp(z, 3, int.MaxValue);
        public RandomSettings RandomSettings => randomSettings;
        
        private int _cachedX;
        private int _cachedY;
        private int _cachedZ;
        private float _cachedPerlinScale;
        private float _cachedPerlinOffsetScale;
        private float _cachedThreshold;
        private int _cachedIterationAmount;

        public bool IsThereAnyChange()
        {
            var result = false;

            if (randomSettings is CustomRandomSettings customRandomSettings)
                result = customRandomSettings.IsThereAnyChange();
            
            if (_cachedX != x ||
                _cachedY != y ||
                _cachedZ != z ||
                Math.Abs(_cachedPerlinScale - perlinScale) > 0.0001f ||
                Math.Abs(_cachedPerlinOffsetScale - perlinOffsetScale) > 0.0001f ||
                Math.Abs(_cachedThreshold - threshold) > 0.0001f ||
                _cachedIterationAmount != iterationAmount)
            {
                result = true;
            }

            _cachedX = x;
            _cachedY = y;
            _cachedZ = z;
            _cachedPerlinScale = perlinScale;
            _cachedPerlinOffsetScale = perlinOffsetScale;
            _cachedThreshold = threshold;
            _cachedIterationAmount = iterationAmount;

            return result;
        }
    }
}