using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapGen.Utilities
{
    public class NoiseDrawer : MonoBehaviour
    {
        public static NoiseDrawer Instance { get; private set; }

        [SerializeField] private float scaleAmount;
        [SerializeField] private RawImage noiseTextureImage;
        [SerializeField] private RectTransform textureRect;

        private void Awake()
        {
            Instance = this;
        }

        public void SetNoiseTexture(float[,] noise, int width, int height)
        {
            textureRect.sizeDelta = Vector2.one * scaleAmount * new Vector2(width, height);
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[x + width * y] = Color.Lerp(Color.black, Color.white, noise[x, y]);
                }
            }
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            noiseTextureImage.texture = texture;
        }
    }
}