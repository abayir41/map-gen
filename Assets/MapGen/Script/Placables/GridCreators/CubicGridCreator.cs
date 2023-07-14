using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables.GridCreators
{
    public class CubicGridCreator : GridCreatorMono
    {
        [SerializeField] protected Vector2Int _cubeLockOffsetX = Vector2Int.up * 5;
        [SerializeField] protected Vector2Int _cubeLockOffsetY = Vector2Int.up * 5;
        [SerializeField] protected Vector2Int _cubeLockOffsetZ = Vector2Int.up * 5;
        
        public override List<Vector3Int> GetGrid()
        {
            var result = new List<Vector3Int>();

            for(var x = _cubeLockOffsetX.x + 1; x < _cubeLockOffsetX.y; x++)
            for(var y = _cubeLockOffsetY.x + 1; y < _cubeLockOffsetY.y; y++)
            for (var z = _cubeLockOffsetZ.x + 1; z < _cubeLockOffsetZ.y; z++)
            {
                result.Add(new Vector3Int(x, y, z));
            }

            return result;
        }
    }
}