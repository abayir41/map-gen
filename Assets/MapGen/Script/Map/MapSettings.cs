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
        [OnChangedCall(nameof(OnPropertyChanged))]
        private int x;
        
        [SerializeField]
        [OnChangedCall(nameof(OnPropertyChanged))]
        private int y;
        
        [SerializeField] 
        [OnChangedCall(nameof(OnPropertyChanged))]
        private int z;
        
        [Header("Random Settings")]
        [SerializeField] 
        [OnChangedCall(nameof(OnPropertyChanged))]
        private RandomSettings randomSettings;
        
        [SerializeField]
        [OnChangedCall(nameof(OnPropertyChanged))]
        private float perlinScale;
        
        [SerializeField]
        [OnChangedCall(nameof(OnPropertyChanged))]
        private float perlinOffsetScale;

        [Range(0,100)][SerializeField]
        [OnChangedCall(nameof(OnPropertyChanged))]
        private float threshold;

        [SerializeField]
        [OnChangedCall(nameof(OnPropertyChanged))]
        private int iterationAmount;
        
        public Action PropertyChanged;

        public float PerlinScale => perlinScale;
        public float PerlinOffsetScale => perlinOffsetScale;
        public float Threshold => threshold;
        public int IterationAmount => iterationAmount;
        
        public int X => x;
        public int Y => y;
        public int Z => z;
        public RandomSettings RandomSettings => randomSettings;

        public void OnPropertyChanged()
        {
            y = Mathf.Clamp(y, 4, int.MaxValue);
            z = Mathf.Clamp(z, 3, int.MaxValue);
            x = Mathf.Clamp(x, 3, int.MaxValue);
            PropertyChanged?.Invoke();
        }
    }
}