using System;
using MapGen.Map.Brushes.NormalMap;
using MapGen.Map.MapEdit;
using UnityEngine;
using UnityEngine.UI;

namespace MapGen.Utilities
{
    public class NoiseDrawer : MonoBehaviour
    {
        [SerializeField] private float scaleAmount;
        [SerializeField] private RawImage noiseTextureImage;
        [SerializeField] private RectTransform textureRect;
        [SerializeField] private Noise.Noise noise;
        [SerializeField] private bool useCustomWidth;
        [SerializeField] private Vector2Int size;

        private WorldEdit WorldEdit => WorldEdit.Instance;

        private void Update()
        {
            if (useCustomWidth)
            {
                SetNoiseTexture(noise.Generate(size.x, size.y), size.x, size.y);
            }
            else
            {
                SetNoiseTexture(
                    noise.Generate(
                        WorldEdit.CurrentSelectCubicBrush.BrushSize.x,
                        WorldEdit.CurrentSelectCubicBrush.BrushSize.z), 
                    WorldEdit.CurrentSelectCubicBrush.BrushSize.x,
                    WorldEdit.CurrentSelectCubicBrush.BrushSize.z);
            }
        }

        private void SetNoiseTexture(float[,] noisee, int width, int height)
        {
            textureRect.sizeDelta = Vector2.one * scaleAmount * new Vector2(width, height);
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[x + width * y] = Color.Lerp(Color.black, Color.white, noisee[x, y]);
                }
            }
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            noiseTextureImage.texture = texture;
        }
    }
}