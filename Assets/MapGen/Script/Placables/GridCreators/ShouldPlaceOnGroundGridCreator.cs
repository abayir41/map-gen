using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables.GridCreators
{
    public class ShouldPlaceOnGroundGridCreator : VisualUserGridCreator
    {
        public override List<Vector3Int> GetGrid()
        {
            var result = new List<Vector3Int>();
            
            foreach(Transform child in _visualsParent)
            {
                if (child.localPosition.y == 0)
                {
                    result.Add(Vector3Int.FloorToInt(child.localPosition));
                }
            }
            
            return result;
        }
    }
}