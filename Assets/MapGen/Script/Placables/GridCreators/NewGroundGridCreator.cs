using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables.GridCreators
{
    public class NewGroundGridCreator : VisualUserGridCreator
    {
        public override List<Vector3Int> GetGrid()
        {
            var result = new List<Vector3Int>();
            foreach(Transform child in _visualsParent)
                result.Add(Vector3Int.FloorToInt(child.localPosition) + Vector3Int.up);

            return result;
        }
    }
}