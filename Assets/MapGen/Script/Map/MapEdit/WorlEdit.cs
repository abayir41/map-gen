using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MapGen.GridSystem;
using MapGen.GridSystem.Obsolete;
using MapGen.Map.MapEdit.Brushes;
using MapGen.Placables;
using UnityEngine;
using UnityEngine.InputSystem;
using Weaver;
using Debug = UnityEngine.Debug;
using Grid = MapGen.GridSystem.Grid;
using GridCell = MapGen.GridSystem.Obsolete.GridCell;

namespace MapGen.Map.MapEdit
{
    public class WorlEdit : MonoBehaviour
    {
        private static WorldCreator WorldCreator => WorldCreator.Instance;
        private static Grid Grid => WorldCreator.Grid;

        [SerializeField] private bool _showGizmos;
        [SerializeField] private CubicBrushSettings _currentSelectCubicBrush;
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;

        public CubicBrushSettings CurrentSelectCubicBrush => _currentSelectCubicBrush;

        private CubicSelectedArea _visualSeenCells;
        private CubicSelectedArea _selectedCells;
        private Plane _selectableCellsGround;
        private Vector3 _selectedPoint;
    

        private void Awake()
        {
            _selectableCellsGround = new Plane(WorldSettings.PLANE_NORMAL, WorldSettings.PLANE_HEIGHT);
        }

        private void Update()
        {
            if(!RayToGridCell(_currentSelectCubicBrush.TargetSelectableGridCells, out var cellPos)) return;


            var cubicSelectedArea = new CubicSelectedArea(
                cellPos - _currentSelectCubicBrush.BrushSize / 2,
                cellPos + _currentSelectCubicBrush.BrushSize / 2);
            
            _visualSeenCells = cubicSelectedArea;
            
            //Debug.Log($"{cubicSelectedArea.StartCellPos}, {cubicSelectedArea.EndCellPos}");
            
            if (IsLeftClicked())
            {
                Paint(cubicSelectedArea);
            }
        }

        public void Paint(CubicSelectedArea cubicSelectedArea)
        {
            var selectedCellPoss = new List<Vector3Int>();
            for (int x = cubicSelectedArea.StartCellPos.x; x < cubicSelectedArea.EndCellPos.x + 1; x++)
            {
                for (int y = cubicSelectedArea.StartCellPos.y; y < cubicSelectedArea.EndCellPos.y + 1; y++)
                {
                    for (int z = cubicSelectedArea.StartCellPos.z; z < cubicSelectedArea.EndCellPos.z + 1; z++)
                    {
                        selectedCellPoss.Add(new Vector3Int(x,y,z));
                    }
                }

            }
                
            WorldCreator.PaintTheBrush(selectedCellPoss);
        }
        
        
        public bool RayToGridCell(LayerMask targetGrid, out Vector3Int cellPos)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = sceneCamera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, sceneCamera.nearClipPlane));

            if (Physics.Raycast(ray, out var result, _maxDistance, targetGrid))
            {
                cellPos = result.collider.GetComponent<SelectableGridCell>().BoundedCell.CellPosition;
                return true;
            }
            
            if (_selectableCellsGround.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                cellPos = WorldCreator.Grid.RealWorldToCellPosition(hitPoint);
                return true;
            }
            
            cellPos = Vector3Int.zero;
            return false;
        }

        public bool IsLeftClicked()
        {
            return Input.GetMouseButton(0);
        }

        private void OnDrawGizmos()
        {
            if(!_showGizmos) return;

            Gizmos.color = Color.green;
            if (_visualSeenCells != null)
            {
                var center = (_visualSeenCells.StartCellPos + _visualSeenCells.EndCellPos) / 2;
                var size = _visualSeenCells.EndCellPos - _visualSeenCells.StartCellPos;
                Gizmos.DrawCube(center, size);
            }
        }
    }
}