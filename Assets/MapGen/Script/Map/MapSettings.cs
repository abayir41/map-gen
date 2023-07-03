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

        [SerializeField] 
        [OnChangedCall(nameof(OnPropertyChanged))]
        private RandomSettings randomSettings;

        public Action PropertyChanged;
        
        public int X => x;
        public int Y => y;
        public int Z => z;
        public RandomSettings RandomSettings => randomSettings;

        public void OnPropertyChanged()
        {
            y = Mathf.Clamp(y, 2, int.MaxValue);
            z = Mathf.Clamp(z, 3, int.MaxValue);
            x = Mathf.Clamp(x, 3, int.MaxValue);
            PropertyChanged?.Invoke();
        }
    }
}