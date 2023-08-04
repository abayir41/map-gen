using System.Collections.Generic;
using MapGen.Command;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Debugger
{
    
    [CreateAssetMenu(fileName = "Debugger", menuName = "MapGen/Brushes/Debugger", order = 0)]
    public class DebuggerBrush : SingleCellEditableBrush
    {
        public override string BrushName => "Debugger";
        protected override int HitBrushHeight => 0;

        private List<Vector3Int> _catchedPlacablePos = new();
        private Color _placableDebugColor = Color.green;
        public override void Update()
        {
            base.Update();

            if (!DidRayHit)
            {
                _catchedPlacablePos.Clear();
                return;
            }
            
            if (!WorldCreator.Grid.IsCellExist(HitPos, out var cell))
            {
                _catchedPlacablePos.Clear();
                return;
            }
            

            if (cell.Item == null)
            {
                _catchedPlacablePos.Clear();
                return;
            }

            _catchedPlacablePos = WorldCreator.Grid.ItemCellsDict[cell.Item].Cells[PlacableCellType.PhysicalVolume].ConvertAll(input => input.CellPosition);
        }

        public override ICommand GetPaintCommand(Vector3Int selectedCells, Grid grid)
        {
            return null;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.color = _placableDebugColor;
            foreach (var selectedCell in _catchedPlacablePos)
            {
                var pos = WorldCreator.Grid.CellPositionToRealWorld(selectedCell);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
    }
}