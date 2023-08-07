using UnityEngine;

namespace MapGen.Noise
{
    [CreateAssetMenu(fileName = "Perlin Noise", menuName = "MapGen/Noise/Perlin", order = 0)]
    public class PerlinNoise : Noise
    {
        [SerializeField] private float noiseScale;
        [SerializeField] private Vector2 noiseOffset;
        
        public override float[,] Generate(int width, int height, int seed)
        {
            var noiseMap = new float[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var samplePosX = x * noiseScale + noiseOffset.x + seed * 10000;
                    var samplePosY = y * noiseScale + noiseOffset.y + seed * 10000;

                    noiseMap[x, y] = Mathf.PerlinNoise(samplePosX, samplePosY);
                }
            }

            return noiseMap;        
        }
    }
}