using UnityEngine;

namespace MapGen.Utilities
{
    public class NoiseGenerator
    {
        public static float[,] Generate(int width, int height, float scale, Vector2 offset)
        {
            var noiseMap = new float[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var samplePosX = x * scale + offset.x;
                    var samplePosY = y * scale + offset.y;

                    noiseMap[x, y] = Mathf.PerlinNoise(samplePosX, samplePosY);
                }
            }

            return noiseMap;
        }
    }
}