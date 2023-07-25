using System.Collections.Generic;
using MapGen.Map.MapEdit;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    
    [CreateAssetMenu(fileName = "Cubic Plain XY", menuName = "MapGen/Brush Areas/Cubic Plain XY Area", order = 0)]
    public class CubicPlainXYBrushArea : BrushArea, IIncreasableBrushArea
    {
        [SerializeField] private Vector2Int _brushSize;

        public override List<Vector3Int> GetBrushArea()
        {
            var minX = -_brushSize.x / 2;
            var minZ = -_brushSize.y / 2;
            var maxX = _brushSize.x / 2;
            var maxZ = _brushSize.y / 2;
            
            if (_brushSize.x % 2 == 0)
            {
                maxX--;
            }
            
            if (_brushSize.y % 2 == 0)
            {
                maxZ--;
            }
            
            var selectedCellPoss = new List<Vector3Int>();
            for (int x = minX; x < maxX + 1; x++)
            {
                for (int z = minZ; z < maxZ + 1; z++)
                {
                    selectedCellPoss.Add(new Vector3Int(x,0,z));
                }
            }

            return selectedCellPoss;
        }

        public void IncreaseArea(int amount)
        {
            _brushSize += Vector2Int.one * amount;
        }

        public void DecreaseArea(int amount)
        {
            _brushSize -= Vector2Int.one * amount;
        }
    }
}