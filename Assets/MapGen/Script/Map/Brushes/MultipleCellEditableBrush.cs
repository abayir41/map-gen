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
        [SerializeField] private BrushArea _brushAreas;
        [SerializeField] private float _gizmoRadius = 0.25f;
        [SerializeField] private Color _visualCellsColor = Color.green;
        
        public BrushArea BrushAreas => _brushAreas;
        private HashSet<Vector3Int> SelectedCells { get; set; } = new();
        protected List<Vector3Int> CurrentlyLookingCells { get; private set; }
        private List<Vector3Int> VisualCells { get; set; }

        public abstract ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid);
        

        public override void Update()
        {
            base.Update();
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                if (BrushAreas is IIncreasableBrushArea increasableBrushArea)
                {
                    increasableBrushArea.IncreaseArea(INCREMENT_BRUSH_AREA);
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                if (BrushAreas is IIncreasableBrushArea increasableBrushArea)
                {
                    increasableBrushArea.DecreaseArea(INCREMENT_BRUSH_AREA);
                }
            }
            
            if(!DidRayHit) return;

            CurrentlyLookingCells = BrushAreas.GetBrushArea(HitPosOffsetted);
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
            
            Gizmos.color = _visualCellsColor;
            foreach (var visualCell in VisualCells)
            {
                var pos = WorldCreator.Grid.CellPositionToRealWorld(visualCell);
                Gizmos.DrawSphere(pos, _gizmoRadius);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
    }
}