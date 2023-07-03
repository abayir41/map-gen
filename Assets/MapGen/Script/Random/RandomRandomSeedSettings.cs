using System;
using UnityEngine;

namespace MapGen.Random
{
    [CreateAssetMenu(fileName = "Random Settings", menuName = "MapGen/Random/Random Seed", order = 0)]
    public class RandomRandomSeedSettings : RandomSettings
    {
        public override int GetSeed() => (int) DateTime.Now.TimeOfDay.TotalMilliseconds;
    }
}