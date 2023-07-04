using MapGen.Random;
using UnityEngine;

namespace MapGen.Noise
{
    
    [CreateAssetMenu(fileName = "Fractal", menuName = "MapGen/Noise/Fractal", order = 0)]
    public class FractalNoise : Noise
    {
        [SerializeField] 
        private RandomSettings randomSettings;
        
        [SerializeField]
        private float noiseScale;
        
        [SerializeField]
        private Vector2 noiseOffset;

        [SerializeField] 
        private FastNoiseLite.FractalType fractalType;

        [SerializeField] 
        private int octaves;
        
        [SerializeField] 
        private int gain;
        
        [SerializeField] 
        private int lacunarity;

        [SerializeField] private float weightedStrength;

        [SerializeField] private float pingPongStrength;

        [SerializeField] private FastNoiseLite.NoiseType noiseType;
        
        public override float[,] Generate(int width, int height)
        {
            var fast = new FastNoiseLite(randomSettings.GetSeed());
            
            fast.SetNoiseType(noiseType);
            fast.SetFractalType(fractalType);
            fast.SetFractalOctaves(octaves);
            fast.SetFractalGain(gain);
            fast.SetFractalLacunarity(lacunarity);
            fast.SetFractalWeightedStrength(weightedStrength);
            fast.SetFractalPingPongStrength(pingPongStrength);
            
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