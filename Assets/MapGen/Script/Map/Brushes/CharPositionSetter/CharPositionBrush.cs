using System.Collections.Generic;
using System.Linq;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    [CreateAssetMenu(fileName = "Char Position Setter", menuName = "MapGen/Brushes/Char Position Setter", order = 0)]
    public class CharPositionBrush : SingleCellEditableBrush
    {
        public override string BrushName => "Char Pos Setter";

        public override void Paint(Vector3Int startPoint, Grid grid)
        {
            FpsState.Instance.CharSpawnPos = startPoint;
        }
        
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            var pos = WorldCreator.Grid.CellPositionToRealWorld(HitPosOffsetted);
            Gizmos.DrawWireCube(pos, Vector3.one);
        }
    }
}