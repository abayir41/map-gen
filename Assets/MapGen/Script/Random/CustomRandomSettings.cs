﻿using UnityEngine;

namespace MapGen.Random
{
    [CreateAssetMenu(fileName = "Random Settings", menuName = "MapGen/Random/Custom Seed", order = 0)]
    public class CustomRandomSettings : RandomSettings
    {
        [SerializeField] private int seed;

        public void SetSeed(int seedInt)
        {
            seed = seedInt;
        }
        
        public override int GetSeed() => seed;
    }
}