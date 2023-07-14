using MapGen.GridSystem;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map.Brushes.GroundBrush
{
    
    [CreateAssetMenu(fileName = "Ground Brush Settings", menuName = "MapGen/Brushes/Ground/Ground Brush Settings", order = 0)]
    public class GroundBrushSettings : ScriptableObject
    {
        public const int GROUND_ROTATION = 0;
        [SerializeField] private Placable ground;
        [SerializeField] private CellLayer _cellLayer = CellLayer.Ground;
        public Placable Ground => ground;

        public CellLayer CellLayer => _cellLayer;
    }
}