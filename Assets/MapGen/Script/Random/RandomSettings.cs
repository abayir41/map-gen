using UnityEngine;

namespace MapGen.Random
{
    public abstract class RandomSettings : ScriptableObject
    {
        public abstract int GetSeed();
    }
}
