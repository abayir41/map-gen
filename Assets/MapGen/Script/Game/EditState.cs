using System.Collections.Generic;
using System.Linq;
using MapGen.Map;
using MapGen.Map.Brushes;
using MapGen.Map.Brushes.BrushAreas;
using UnityEngine;
using Plugins.Editor;
using TMPro;
using UnityEngine.InputSystem;

namespace MapGen
{
    public class EditState : State
    {
        [Header("State Transition Required")]
        [SerializeField] private FpsState _fpsState;
        [SerializeField] private GameObject _camera;
        [SerializeField] private GameObject _worldCreator;
        [SerializeField] private GameObject _canvas;
        
        [Header("Visual Debug")]
        [SerializeField] private bool _showGizmos;
        [SerializeField] private TextMeshProUGUI yOffsetText;
        [SerializeField] private TextMeshProUGUI brushName;
        [SerializeField] private TextMeshProUGUI brushAreaName;
        
        [Header("Editing")]
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;

        private static WorldCreator WorldCreator => WorldCreator.Instance;
        private GameManager GameManager => GameManager.Instance;
        private BrushSelector BrushSelector => BrushSelector.Instance;

        
        private int _selectedAreYOffset;
        private HashSet<Vector3Int> _selectedCells = new();
        private Plane _selectableCellsGround;
        private List<Vector3Int> _currentlyLookingArea;

        private void Awake()
        {
            _selectableCellsGround = new Plane(WorldSettings.PlaneStartNormal, WorldSettings.PlaneStartHeight);
        }

        private void Start()
        {
            yOffsetText.text = "Brush Y Offset: " + _selectedAreYOffset + " - Mouse Wheel";
            brushName.text = "Brush: " + BrushSelector.CurrentBrush.BrushName + " - Q or E";
            brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
        }

        private void Update()
        {
            BrushOperations();
            
            if (!RayToGridCell(out var cellPos))
            {
                _currentlyLookingArea = null;
                return;
            }

            var brushArea = BrushSelector.CurrentBrushArea.GetBrushArea();
            var currentlyLookingArea = ApplyOffsetToPoss(brushArea, cellPos + Vector3Int.up * _selectedAreYOffset);
            
            _currentlyLookingArea = currentlyLookingArea;
            
            
            if (Input.GetMouseButton(0) && BrushSelector.CurrentBrush.SupportMultipleCells && Input.GetKey(KeyCode.LeftShift))
            {
                foreach (var cellPosition in currentlyLookingArea)
                {
                    if (!_selectedCells.Contains(cellPosition))
                    {
                        _selectedCells.Add(cellPosition);
                    }
                }
            }
            else if (Input.GetMouseButton(0) && BrushSelector.CurrentBrush.SupportMultipleCells && Input.GetKey(KeyCode.LeftControl))
            {
                foreach (var cellPosition in currentlyLookingArea)
                {
                    if (_selectedCells.Contains(cellPosition))
                    {
                        _selectedCells.Remove(cellPosition);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (_selectedCells.Count == 0)
                {
                    WorldCreator.PaintTheBrush(_currentlyLookingArea);
                }
                else
                {
                    WorldCreator.PaintTheBrush(_selectedCells.ToList());
                }
                
                ResetSelectedArea();
            }
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

        private void BrushOperations()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                if (Input.GetKey(KeyCode.LeftControl) && BrushSelector.CurrentBrushArea is IIncreasableBrushArea increasableBrushArea)
                {
                    increasableBrushArea.IncreaseArea(1);
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    BrushSelector.NextBrushArea();
                    brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
                }
                else
                {
                    _selectedAreYOffset++;
                    yOffsetText.text = "Brush Y Offset: " + _selectedAreYOffset + " - Mouse Wheel";
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                if (Input.GetKey(KeyCode.LeftControl) && BrushSelector.CurrentBrushArea is IIncreasableBrushArea increasableBrushArea)
                {
                    increasableBrushArea.DecreaseArea(1);
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    BrushSelector.PreviousBrushArea();
                    brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
                }
                else
                {
                    _selectedAreYOffset--;
                    yOffsetText.text = "Brush Y Offset: " + _selectedAreYOffset + " - Mouse Wheel";
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameManager.SwitchState(this, _fpsState);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                BrushSelector.PreviousBrush();
                brushName.text = "Brush: " + BrushSelector.CurrentBrush.BrushName + " - Q or E";
                brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
                
                if(!BrushSelector.CurrentBrush.SupportMultipleCells)
                    ResetSelectedArea();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                BrushSelector.NextBrush();
                brushName.text = "Brush: " + BrushSelector.CurrentBrush.BrushName + " - Q or E";
                brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
                
                if(!BrushSelector.CurrentBrush.SupportMultipleCells)
                    ResetSelectedArea();
            }
        }
        
        private void ResetSelectedArea()
        {
            _selectedCells.Clear();
        }
        
        #region Brush Implementation

        public override void OnStateEnter()
        {
            GizmoToggle.ToggleGizmos(true);
            _camera.SetActive(true);
            _worldCreator.SetActive(true);
            _canvas.SetActive(true);
        }

        public override void OnStateExit()
        {
            GizmoToggle.ToggleGizmos(false);
            _camera.SetActive(false);
            _worldCreator.SetActive(false);
            _canvas.SetActive(false);
        }

        #endregion

        private void OnDrawGizmos()
        {
            if(!_showGizmos) return;

            Gizmos.color = Color.blue;
            foreach (var selectedCell in _selectedCells)
            {
                Gizmos.DrawWireCube(selectedCell, WorldSettings.GridCellRealWorldSize);
            }


            if (_currentlyLookingArea != null)
            {
                Gizmos.color = Color.green;
                foreach (var currentlyLookingCell in _currentlyLookingArea)
                {
                    Gizmos.DrawWireCube(currentlyLookingCell, WorldSettings.GridCellRealWorldSize);
                }
            }
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(_fpsState.CharSpawnPos, WorldSettings.GridCellRealWorldSize);
        }
        
        public List<Vector3Int> ApplyOffsetToPoss(List<Vector3Int> poss, Vector3Int offset)
        {
            return poss.ConvertAll(input => input + offset);
        }
    }
}