using UnityEngine;

namespace MapGen.Placables
{
    public class PlacableNormalGrid : PlacableGrid
    {
        [SerializeField] private PlacableCellType _placableCellType;

        public override PlacableCellType GetCellType()
        {
            return _placableCellType;
        }
    }
}