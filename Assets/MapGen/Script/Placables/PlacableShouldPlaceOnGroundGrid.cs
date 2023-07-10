using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public class PlacableShouldPlaceOnGroundGrid : PlacableGrid
    {
        public override PlacableCellType GetCellType()
        {
            return PlacableCellType.ShouldPlaceOnGround;
        }

        protected override void VisualAlgorithm()
        {
            _cellPositions = new List<Vector3Int>();
            foreach(Transform child in _visualsParent)
                if(child.localPosition.y == 0)
                    _cellPositions.Add(Vector3Int.FloorToInt(child.localPosition));
        }
    }
}