using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    
    [CreateAssetMenu(fileName = "Cubic Plain XYZ", menuName = "MapGen/Brush Areas/Cubic XYZ Area", order = 0)]
    public class CubicXYZBrushArea : BrushArea, IIncreasableBrushArea
    {
        [SerializeField] private Vector3Int _brushSize;

        public override List<Vector3Int> GetBrushArea()
        {
            var minX = -_brushSize.x / 2;
            var minZ = -_brushSize.z / 2;
            var minY = -_brushSize.y / 2;
            var maxX = _brushSize.x / 2;
            var maxZ = _brushSize.y / 2;
            var maxY = _brushSize.y / 2;

            if (_brushSize.x % 2 == 0)
            {
                maxX--;
            }

            if (_brushSize.z % 2 == 0)
            {
                maxZ--;
            }
            
            if (_brushSize.y % 2 == 0)
            {
                maxY--;
            }
            
            var selectedCellPoss = new List<Vector3Int>();
            for (int x = minX; x < maxX + 1; x++)
            {
                for (int y = minY; y < maxY + 1; y++)
                {
                    for (int z = minZ; z < maxZ + 1; z++)
                    {
                        selectedCellPoss.Add(new Vector3Int(x,y,z));
                    }
                }
            }

            return selectedCellPoss;
        }

        public void IncreaseArea(int amount)
        {
            _brushSize += Vector3Int.one * amount;
        }

        public void DecreaseArea(int amount)
        {
            _brushSize -= Vector3Int.one * amount;
        }
    }
}