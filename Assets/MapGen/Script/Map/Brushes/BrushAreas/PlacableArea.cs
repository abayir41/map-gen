using System.Collections.Generic;
using System.Linq;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map.Brushes.BrushAreas
{
    
    [CreateAssetMenu(fileName = "Placable", menuName = "MapGen/Brush Areas/Placable", order = 0)]
    public class PlacableArea : BrushArea
    {
        [SerializeField] private Placable _placable;
        [SerializeField] private PlacableCellType _cellTypeToBeShown;


        public Placable Placable => _placable;


        public override List<Vector3Int> GetBrushArea(Vector3Int startPoint)
        {
            var placableRequired =
                _placable.Grids.FirstOrDefault(grid => grid.PlacableCellType == _cellTypeToBeShown);

            if (placableRequired == null) return new List<Vector3Int>();

            if (WorldCreator.Instance.Grid.IsPlacableSuitable(startPoint, _placable, 0))
            {
                _areaColor = Color.green;
            }
            else
            {
                _areaColor = Color.red;
            }
            
            
            var result = ApplyOffsetToPoss(placableRequired.CellPositions, startPoint);
            result = ApplyOffsetToPoss(result, -_placable.Origin);
            return result;
        }
    }
}