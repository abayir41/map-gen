using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public class PlacableNewGroundGrid : PlacableGrid
    {
        public override PlacableCellType GetCellType()
        {
            return PlacableCellType.NewGround;
        }

        protected override void VisualAlgorithm()
        {
            _cellPositions = new List<Vector3Int>();
            foreach(Transform child in _visualsParent)
                _cellPositions.Add(Vector3Int.FloorToInt(child.localPosition) + Vector3Int.up);
        }
    }
}