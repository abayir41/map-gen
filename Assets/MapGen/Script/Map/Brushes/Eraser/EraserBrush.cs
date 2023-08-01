using System;
using System.Collections.Generic;
using MapGen.Map.Brushes.BrushAreas;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Eraser
{ 
    [CreateAssetMenu(fileName = "Eraser", menuName = "MapGen/Brushes/Eraser", order = 0)]
    public class EraserBrush : MultipleCellEditableBrush
    {
        public override string BrushName => "Eraser";
        
        private List<Vector3Int> _catchedPlacablePos = new();
        private Color _placableDebugColor = Color.red;
        private float _radius = 0.25f;
        
        public override void Update()
        {
            base.Update();
            
            if (!DidRayHit)
            {
                _catchedPlacablePos.Clear();
                return;
            }
            
            _catchedPlacablePos.Clear();
            
            foreach (var currentlyLookingCell in CurrentlyLookingCells)
            {
                if (!WorldCreator.Grid.IsCellExist(currentlyLookingCell, out var cell))
                {
                    continue;
                }
            

                if (cell.Item == null)
                {
                    continue;
                }

                _catchedPlacablePos.AddRange(WorldCreator.Grid.ItemCellsDict[cell.Item].PhysicalCells.ConvertAll(input => input.CellPosition));
            }
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.color = _placableDebugColor;
            foreach (var selectedCell in _catchedPlacablePos)
            {
                var pos = WorldCreator.Grid.CellPositionToRealWorld(selectedCell);
                Gizmos.DrawSphere(pos, _radius);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            foreach (var selectedCell in selectedCells)
            {
                if (grid.IsCellExist(selectedCell, out var cell) && cell.Item != null)
                {
                    WorldCreator.Instance.DestroyItem(cell.Item);
                }
            }
        }
    }
}