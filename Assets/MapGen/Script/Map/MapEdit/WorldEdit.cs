using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map.MapEdit.Brushes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MapGen.Map.MapEdit
{
    public class WorldEdit : MonoBehaviour
    {
        public static WorldEdit Instance { get; private set; }
        
        private static WorldCreator WorldCreator => WorldCreator.Instance;

        [SerializeField] private bool _showGizmos;
        [SerializeField] private CubicBrushSettings _currentSelectCubicBrush;
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;

        public CubicBrushSettings CurrentSelectCubicBrush => _currentSelectCubicBrush;
        public int SelectedAreYOffset;
        

        private CubicSelectedArea _visualSeenCells;
        private Plane _selectableCellsGround;

        private void Awake()
        {
            Instance = this;
            _selectableCellsGround = new Plane(WorldSettings.PlaneNormal, WorldSettings.PlaneHeight);
        }

        private void Update()
        {
            if (!RayToGridCell(_currentSelectCubicBrush.TargetSelectableGridCells, out var cellPos))
            {
                _visualSeenCells = null;
                return;
            }

            var cubicSelectedArea = new CubicSelectedArea(
                cellPos + Vector3Int.up * SelectedAreYOffset - _currentSelectCubicBrush.BrushSize / 2,
                cellPos + Vector3Int.up * SelectedAreYOffset + _currentSelectCubicBrush.BrushSize / 2);
            
            _visualSeenCells = cubicSelectedArea;
            
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
            
            if (_selectableCellsGround.Raycast(ray, out var enter) && enter < _maxDistance)
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
            return Input.GetMouseButtonDown(0);
        }

        private void OnDrawGizmos()
        {
            if(!_showGizmos) return;

            Gizmos.color = Color.green;
            if (_visualSeenCells != null)
            {
                var center = (_visualSeenCells.StartCellPos + _visualSeenCells.EndCellPos) / 2;
                var size = _visualSeenCells.EndCellPos - _visualSeenCells.StartCellPos + Vector3Int.one;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}