using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map.Brushes;
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
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;

        private static BrushSelector BrushSelector => BrushSelector.Instance;

        
        public int SelectedAreYOffset;
        

        private List<Vector3Int> _visualSeenCells;
        private Plane _selectableCellsGround;

        private void Awake()
        {
            Instance = this;
            _selectableCellsGround = new Plane(WorldSettings.PlaneNormal, WorldSettings.PlaneHeight);
        }

        private void Update()
        {
            if (!RayToGridCell(out var cellPos))
            {
                _visualSeenCells = null;
                return;
            }

            var brushArea = BrushSelector.CurrentBrushArea.GetBrushArea();
            var fixedArea = ApplyOffsetToPoss(brushArea, cellPos + Vector3Int.up * SelectedAreYOffset);
            
            _visualSeenCells = fixedArea;
            
            if (IsLeftClicked())
            {
                WorldCreator.PaintTheBrush(fixedArea);
            }
        }

        public List<Vector3Int> ApplyOffsetToPoss(List<Vector3Int> poss, Vector3Int offset)
        {
            return poss.ConvertAll(input => input + offset);
        }

        public bool RayToGridCell(out Vector3Int cellPos)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = sceneCamera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, sceneCamera.nearClipPlane));

            /*
            if (Physics.Raycast(ray, out var result, _maxDistance))
            {
                cellPos = result.collider.GetComponent<SelectableGridCell>().BoundedCell.CellPosition;
                return true;
            }
            */
            
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
                foreach (var visualSeenCell in _visualSeenCells)
                {
                    Gizmos.DrawWireCube(visualSeenCell, Vector3.one);
                }
            }
        }
    }
}