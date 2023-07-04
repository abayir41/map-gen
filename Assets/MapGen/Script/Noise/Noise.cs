using UnityEngine;

namespace MapGen.Noise
{
    
    public abstract class Noise : ScriptableObject
    {
        public abstract float[,] Generate(int width, int height);
    }
}