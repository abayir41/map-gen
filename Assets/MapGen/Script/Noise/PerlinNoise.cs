using UnityEngine;

namespace MapGen.Noise
{
    [CreateAssetMenu(fileName = "Perlin Noise", menuName = "MapGen/Noise/Perlin", order = 0)]
    public class PerlinNoise : Noise
    {
        public override float[,] Generate(int width, int height)
        {
            var noiseMap = new float[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var samplePosX = x * noiseScale + noiseOffset.x;
                    var samplePosY = y * noiseScale + noiseOffset.y;

                    noiseMap[x, y] = Mathf.PerlinNoise(samplePosX, samplePosY);
                }
            }

            return noiseMap;        
        }
    }
}