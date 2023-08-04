using System;
using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Eraser
{ 
    [CreateAssetMenu(fileName = "Eraser", menuName = "MapGen/Brushes/Eraser", order = 0)]
    public class EraserBrush : MultipleCellEditableBrush
    {
        public override string BrushName => "Eraser";
        protected override int HitBrushHeight => 0;

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

                _catchedPlacablePos.AddRange(WorldCreator.Grid.ItemCellsDict[cell.Item].Cells[PlacableCellType.PhysicalVolume].ConvertAll(input => input.CellPosition));
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

        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new EraserCommand(WorldCreator.Instance, this, selectedCells, grid);
        }

        public override List<SpawnData> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var result = new List<SpawnData>();
            foreach (var selectedCell in selectedCells)
            {
                if (grid.IsCellExist(selectedCell, out var cell) && cell.Item != null)
                {
                    var data = cell.Item.SpawnData;
                    result.Add(data);
                    WorldCreator.Instance.DestroyItem(cell.Item);
                }
            }

            return result;
        }
    }
}