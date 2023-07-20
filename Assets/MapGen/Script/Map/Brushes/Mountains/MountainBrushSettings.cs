using MapGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace MapGen.Map.Brushes.Mountains
{
    [CreateAssetMenu(fileName = "Mountains Brush Settings", menuName = "MapGen/Brushes/Mountains/Brush Settings", order = 0)]
    public class MountainBrushSettings : ScriptableObject
    {
        [SerializeField] private RandomSettings randomSettings;
        [SerializeField] private Placable ground;
        [SerializeField] private Noise.Noise _mountainPlacementNoise;
        [SerializeField] private float groundHeightFactor;
        [SerializeField] private float groundMoveDownFactor;
        public const int GROUND_ROTATION = 0;

        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise MountainPlacementNoise => _mountainPlacementNoise;
        public float GroundHeightFactor => groundHeightFactor;
        public float GroundMoveDownFactor => groundMoveDownFactor;
        public Placable Ground => ground;
    }
}