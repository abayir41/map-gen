using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map.MapEdit.Brushes;
using UnityEngine;
using UnityEngine.InputSystem;
using Weaver;
using Debug = UnityEngine.Debug;

namespace MapGen.Map.MapEdit
{
    public class WorlEdit : MonoBehaviour
    {
        private static WorldCreator WorldCreator => WorldCreator.Instance;
        private static Grid Grid => WorldCreator.Grid;

        [SerializeField] private bool _showGizmos;
        [SerializeField] private BrushSettings _currentSelectBrush;
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;

        public BrushSettings CurrentSelectBrush => _currentSelectBrush;

        private List<GridCell> _visualSeenCells = new();
        private List<GridCell> _selectedCells = new();

        private void Update()
        {
            if(!RayToGridCell(out var gridCell, _currentSelectBrush.TargetSelectableGridCells)) return;

            
            if (Grid.GetWithNeighbor(gridCell.BoundedCell, _currentSelectBrush.BrushSizeX, _currentSelectBrush.BrushSizeY,
                    _currentSelectBrush.BrushSizeZ, out var visuallySelectedCells))
            {
                _visualSeenCells = visuallySelectedCells;
                if (IsLeftClicked())
                {
                    _selectedCells = _currentSelectBrush.GetWantedAreaAsGridCell(visuallySelectedCells, Grid);
                    WorldCreator.PaintTheBrush(_selectedCells);
                }
            }
            else
            {
                _visualSeenCells = null;
                _selectedCells = null;
            }

            
        }
        
        
        public bool RayToGridCell(out SelectableGridCell gridCell, LayerMask targetGrid)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = sceneCamera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, sceneCamera.nearClipPlane));

            if (Physics.Raycast(ray, out var result, _maxDistance, targetGrid))
            {
                gridCell = result.collider.GetComponent<SelectableGridCell>();
                return true;
            }
            else
            {
                gridCell = null;
                return false;
            }
        }

        public bool IsLeftClicked()
        {
            return Input.GetMouseButton(0);
        }

        private void OnDrawGizmos()
        {
            if(!_showGizmos) return;

            Gizmos.color = Color.green;
            if (_visualSeenCells != null && _visualSeenCells.Count > 0)
            {
                Gizmos.DrawWireCube(
                    new Vector3(
                        (float)_visualSeenCells.Average(cell => cell.Position.x),
                        (float)_visualSeenCells.Average(cell => cell.Position.y),
                        (float)_visualSeenCells.Average(cell => cell.Position.z)
                    ),
                    new Vector3(
                        _visualSeenCells.Max(cell => cell.Position.x) - _visualSeenCells.Min(cell => cell.Position.x),
                        _visualSeenCells.Max(cell => cell.Position.y) - _visualSeenCells.Min(cell => cell.Position.y),
                        _visualSeenCells.Max(cell => cell.Position.z) - _visualSeenCells.Min(cell => cell.Position.z)
                    ));
            }
        }
    }
}