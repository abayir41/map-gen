using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map;
using MapGen.Map.Brushes;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Utilities;
using UnityEngine;
using Plugins.Editor;
using TMPro;
using UnityEngine.InputSystem;

namespace MapGen
{
    public class EditState : State
    {
        public static EditState Instance { get; private set; }
        
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
        [SerializeField] private TextMeshProUGUI placableDescription;
        
        [Header("Editing")]
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;
        [SerializeField] private LayerMask _selectableGridMask;
        [SerializeField] private EndlessList<Brush> _brushes;

        private static WorldCreator WorldCreator => WorldCreator.Instance;
        private GameManager GameManager => GameManager.Instance;

        
        private int _selectedAreYOffset = 1;
        private Plane _selectableCellsGround;


        public int SelectedAreYOffset => _selectedAreYOffset;

        private void Awake()
        {
            Instance = this;
            _selectableCellsGround = new Plane(WorldSettings.PlaneStartNormal, WorldSettings.PlaneStartHeight);
        }

        private void Update()
        { 
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _brushes.PreviousItem();
                if (_brushes.CurrentItem is PlacableSpawner)
                {
                    placableDescription.text = "Left-Right Arrow to change placable, Up-Down Arrow to rotate";
                }
                else
                {
                    placableDescription.text = "";
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _brushes.NextItem();
                if (_brushes.CurrentItem is PlacableSpawner)
                {
                    placableDescription.text = "Left-Right Arrow to change placable, Up-Down Arrow to rotate";
                }
                else
                {
                    placableDescription.text = "";
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameManager.SwitchState(this, _fpsState);
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) // forward
            {
                _selectedAreYOffset++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) // backwards
            {
                _selectedAreYOffset--;
            }
            
            
            yOffsetText.text = "Brush Y Offset: " + _selectedAreYOffset + " - Mouse Wheel";
            brushName.text = "Brush: " + _brushes.CurrentItem.BrushName + " - Q or E";

            if (_brushes.CurrentItem is MultipleCellEditableBrush multipleCellEditableBrush)
            {
                brushAreaName.text = "Area: " + multipleCellEditableBrush.BrushAreas.CurrentItem.name + " - SHIFT + Mouse Wheel";
            }
            else
            {
                brushAreaName.text = "";
            }
            
            _brushes.CurrentItem.Update();
        }
        
        public bool RayToGridCell(out Vector3Int cellPos)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = sceneCamera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, sceneCamera.nearClipPlane));

            
            if (Physics.Raycast(ray, out var result, _maxDistance, _selectableGridMask))
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
        
        
        #region State Implementation

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

            _brushes.CurrentItem.OnDrawGizmos();
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(_fpsState.CharSpawnPos, WorldSettings.GridCellRealWorldSize);
        }
    }
}