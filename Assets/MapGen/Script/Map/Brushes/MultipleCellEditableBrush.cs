using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Utilities;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class MultipleCellEditableBrush : Brush
    {
        private const int INCREMENT_BRUSH_AREA = 1;
        
        [SerializeField] private Color _selectedCellsColor = Color.blue;
        [SerializeField] private EndlessList<BrushArea> _brushAreas;
        
        public virtual EndlessList<BrushArea> BrushAreas => _brushAreas;
        private HashSet<Vector3Int> SelectedCells { get; set; } = new();
        protected List<Vector3Int> CurrentlyLookingCells { get; private set; }

        public abstract ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid);

        public override void Update()
        {
            base.Update();
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                if (Input.GetKey(KeyCode.LeftControl) && BrushAreas.CurrentItem is IIncreasableBrushArea increasableBrushArea)
                {
                    increasableBrushArea.IncreaseArea(INCREMENT_BRUSH_AREA);
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    BrushAreas.NextItem();
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                if (Input.GetKey(KeyCode.LeftControl) && BrushAreas.CurrentItem is IIncreasableBrushArea increasableBrushArea)
                {
                    increasableBrushArea.DecreaseArea(INCREMENT_BRUSH_AREA);
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    BrushAreas.PreviousItem();
                }
            }
            
            if(!DidRayHit) return;

            CurrentlyLookingCells = BrushAreas.CurrentItem.GetBrushArea(HitPosOffsetted);
            VisualCells = CurrentlyLookingCells;
            
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift))
            {
                foreach (var cellPosition in CurrentlyLookingCells)
                {
                    if (!SelectedCells.Contains(cellPosition))
                    {
                        SelectedCells.Add(cellPosition);
                    }
                }
            }
            else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
            {
                foreach (var cellPosition in CurrentlyLookingCells)
                {
                    if (SelectedCells.Contains(cellPosition))
                    {
                        SelectedCells.Remove(cellPosition);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (SelectedCells.Count == 0)
                {
                    var command = GetPaintCommand(CurrentlyLookingCells, WorldCreator.Grid);
                    CommandManager.Instance.RunCommand(command);
                }
                else
                {
                    var command =  GetPaintCommand(SelectedCells.ToList(), WorldCreator.Grid);
                    CommandManager.Instance.RunCommand(command);
                }
                
                ResetSelectedArea();
            }
        }

        private void ResetSelectedArea()
        {
            SelectedCells.Clear();
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.color = _selectedCellsColor;
            foreach (var selectedCell in SelectedCells)
            {
                var pos = WorldCreator.Grid.CellPositionToRealWorld(selectedCell);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
    }
}