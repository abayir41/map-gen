using UnityEngine;

namespace MapGen.Noise
{
    public abstract class Noise : ScriptableObject
    {
        [SerializeField]
        public float noiseScale;
        
        [SerializeField]
        public Vector2 noiseOffset;

        public abstract float[,] Generate(int width, int height);
    }
}