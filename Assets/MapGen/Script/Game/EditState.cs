using System;
using MapGen.GridSystem;
using MapGen.Map;
using MapGen.Map.Brushes;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Utilities;
using UnityEngine;
using Plugins.Editor;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        
        [Header("UI - Gizmos")]
        [SerializeField] private bool _showGizmos;
        [SerializeField] private TextMeshProUGUI brushName;
        [SerializeField] private Image _brushIcon;
        [SerializeField] private GameObject _increaseBrushAreaParent;
        [SerializeField] private TextMeshProUGUI _brushAreaAmount;
        [SerializeField] private GameObject placableParent;
        [SerializeField] private TextMeshProUGUI currentPlacable;
        [SerializeField] private GameObject rotationParent;
        [SerializeField] private TextMeshProUGUI currentRotation;
        
        [Header("Editing")]
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private int _maxDistance;
        [SerializeField] private LayerMask _selectableGridMask;
        [SerializeField] private EndlessList<Brush> _brushes;
        [SerializeField] private int _increaseAmount = 1;

        private static WorldCreator WorldCreator => WorldCreator.Instance;
        private GameManager GameManager => GameManager.Instance;

        
        private int _selectedAreYOffset;
        private Plane _selectableCellsGround;


        public int SelectedAreYOffset => _selectedAreYOffset;

        private void Awake()
        {
            Instance = this;
            _selectableCellsGround = new Plane(WorldSettings.PlaneStartNormal, WorldSettings.PlaneStartHeight);
        }

        private void Start()
        {
            brushName.text = "Brush: " + _brushes.CurrentItem.BrushName + " - Q or E";
            _brushIcon.sprite = _brushes.CurrentItem.BrushIcon;
        }

        private void Update()
        {
            if (_brushes.CurrentItem is MultipleCellEditableBrush multipleCellEditableBrush &&
                multipleCellEditableBrush.BrushAreas is IIncreasableBrushArea brush)
            {
                _increaseBrushAreaParent.SetActive(true);
                _brushAreaAmount.text = brush.BrushSize.ToString();
            }
            else
            {
                _increaseBrushAreaParent.SetActive(false);
            }

            if (_brushes.CurrentItem is PlacableSpawner placableSpawner)
            { 
                placableParent.SetActive(true);
                currentPlacable.text = placableSpawner.CurrentPlacableName;
                
                rotationParent.SetActive(true);
                currentRotation.text = placableSpawner.Rotation.ToString();
            }
            else
            {
                placableParent.SetActive(false);
                rotationParent.SetActive(false);
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PreviousBrush();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                NextBrush();
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchToFps();
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && Input.GetKey(KeyCode.LeftControl)) // forward
            {
                _selectedAreYOffset++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && Input.GetKey(KeyCode.LeftControl)) // backwards
            {
                _selectedAreYOffset--;
            }


            _brushes.CurrentItem.Update();
        }

        public void SwitchToFps()
        {
            GameManager.SwitchState(this, _fpsState);
        }

        public void NextBrush()
        {
            _brushes.NextItem();
            brushName.text = "Brush: " + _brushes.CurrentItem.BrushName + " - Q or E";
            _brushIcon.sprite = _brushes.CurrentItem.BrushIcon;
        }

        public void PreviousBrush()
        {
            _brushes.PreviousItem();
            brushName.text = "Brush: " + _brushes.CurrentItem.BrushName + " - Q or E";
            _brushIcon.sprite = _brushes.CurrentItem.BrushIcon;
        }

        public void IncreaseBrushArea()
        {
            var multipleCellEditableBrush = _brushes.CurrentItem as MultipleCellEditableBrush;
            var brush = multipleCellEditableBrush!.BrushAreas as IIncreasableBrushArea;
            brush!.IncreaseArea(_increaseAmount);
        }
        
        public void DecreaseBrushArea()
        {
            var multipleCellEditableBrush = _brushes.CurrentItem as MultipleCellEditableBrush;
            var brush = multipleCellEditableBrush!.BrushAreas as IIncreasableBrushArea;
            brush!.DecreaseArea(_increaseAmount);
        }

        public void NextRotatePlacable()
        {
            var spawner = _brushes.CurrentItem as PlacableSpawner;
            spawner.NextRotatePlacable();
        }

        public void PreviousRotatePlacable()
        {
            var spawner = _brushes.CurrentItem as PlacableSpawner;
            spawner.PreviousRotatePlacable();
        }

        public void NextPlacable()
        {
            var spawner = _brushes.CurrentItem as PlacableSpawner;
            spawner.NextPlacable();
        }

        public void PreviousPlacable()
        {
            var spawner = _brushes.CurrentItem as PlacableSpawner;
            spawner.PreviousPlacable();
        }
        
        public bool RayToGridCell(out Vector3Int cellPos)
        {

            if (IsMouseOverUI())
            {
                cellPos = Vector3Int.zero;
                return false;
            }
            
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

        public bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
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