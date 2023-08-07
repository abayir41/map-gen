using MapGen.Random;
using UnityEngine;

namespace MapGen.Noise
{
    [CreateAssetMenu(fileName = "Cellular Noise", menuName = "MapGen/Noise/Cellular", order = 0)]
    public class CellularNoise : Noise
    {
        [SerializeField] private float noiseScale;
        [SerializeField] private Vector2 noiseOffset;
        
        public override float[,] Generate(int width, int height, int seed)
        {
            var fast = new FastNoiseLite(seed);
            fast.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            
            var noiseMap = new float[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var samplePosX = x * noiseScale + noiseOffset.x;
                    var samplePosY = y * noiseScale + noiseOffset.y;

                    noiseMap[x, y] = fast.GetNoise(samplePosX, samplePosY);
                }
            }

            return noiseMap;
        }
    }
}