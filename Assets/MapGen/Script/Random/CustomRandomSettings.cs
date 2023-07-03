using System;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.Random
{
    [CreateAssetMenu(fileName = "Random Settings", menuName = "MapGen/Random/Custom Seed", order = 0)]
    public class CustomRandomSettings : RandomSettings
    {
        [SerializeField] 
        [OnChangedCall(nameof(OnPropertyChanged))]
        private int seed;

        public Action PropertyChanged;
        
        public override int GetSeed() => seed;
        
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke();
        }
    }
}