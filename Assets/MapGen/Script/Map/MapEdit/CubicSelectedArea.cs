using UnityEngine;

namespace MapGen.Map.MapEdit
{
    public class CubicSelectedArea
    {
        public Vector3Int StartCellPos { get; }
        public Vector3Int EndCellPos { get; }
        
        public CubicSelectedArea(Vector3Int startCellPos, Vector3Int endCellPos)
        {
            StartCellPos = startCellPos;
            EndCellPos = endCellPos;
        }
    }
}