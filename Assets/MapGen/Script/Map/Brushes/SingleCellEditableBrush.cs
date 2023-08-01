using System.Collections.Generic;
using System.Linq;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Utilities;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class SingleCellEditableBrush : Brush
    {
        public abstract void Paint(Vector3Int selectedCells, Grid grid);
        public override void Update()
        {
            base.Update();
            
            if(!DidRayHit) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                Paint(HitPos, WorldCreator.Grid);
            }
        }
    }
}