using UnityEngine;

namespace MapGen.Map.MapEdit.Brushes
{
    
    [CreateAssetMenu(fileName = "Cubic Brush Settings", menuName = "MapGen/Map Edit/Brushes/Cubic Brush Settings", order = 0)]
    public class CubicBrushSettings : ScriptableObject
    {
        [SerializeField] private LayerMask _targetSelectableGridCells;
        [SerializeField] private Vector3Int _brushSize;

        public LayerMask TargetSelectableGridCells => _targetSelectableGridCells;
        public Vector3Int BrushSize => _brushSize;
    }
}