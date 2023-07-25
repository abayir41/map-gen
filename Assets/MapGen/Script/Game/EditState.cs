using System;
using MapGen.Map.Brushes;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Map.MapEdit;
using UnityEngine;
using Plugins.Editor;
using TMPro;

namespace MapGen
{
    public class EditState : State
    {
        [SerializeField] private FpsState _fpsState;
        [SerializeField] private GameObject _camera;
        [SerializeField] private GameObject _worldCreator;
        [SerializeField] private GameObject _gridSelector;
        [SerializeField] private GameObject _canvas;
        [SerializeField] private TextMeshProUGUI yOffsetText;
        [SerializeField] private TextMeshProUGUI brushName;
        [SerializeField] private TextMeshProUGUI brushAreaName;
        
        private GameManager GameManager => GameManager.Instance;
        private WorldEdit WorldEdit => WorldEdit.Instance;
        private BrushSelector BrushSelector => BrushSelector.Instance;

        private void Start()
        {
            yOffsetText.text = "Brush Y Offset: " + WorldEdit.SelectedAreYOffset + " - Mouse Wheel";
            brushName.text = "Brush: " + BrushSelector.CurrentBrush.BrushName + " - Q or E";
            brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
        }

        private void Update()
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
                    WorldEdit.SelectedAreYOffset++;
                    yOffsetText.text = "Brush Y Offset: " + WorldEdit.SelectedAreYOffset + " - Mouse Wheel";
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
                    WorldEdit.SelectedAreYOffset--;
                    yOffsetText.text = "Brush Y Offset: " + WorldEdit.SelectedAreYOffset + " - Mouse Wheel";
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
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                BrushSelector.NextBrush();
                brushName.text = "Brush: " + BrushSelector.CurrentBrush.BrushName + " - Q or E";
                brushAreaName.text = "Area: " + BrushSelector.CurrentBrushArea.name + " - SHIFT + Mouse Wheel";
            }
        }

        public override void OnStateEnter()
        {
            GizmoToggle.ToggleGizmos(true);
            _camera.SetActive(true);
            _worldCreator.SetActive(true);
            _gridSelector.SetActive(true);
            _canvas.SetActive(true);
        }

        public override void OnStateExit()
        {
            GizmoToggle.ToggleGizmos(false);
            _camera.SetActive(false);
            _gridSelector.SetActive(false);
            _worldCreator.SetActive(false);
            _canvas.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(_fpsState.CharSpawnPos, Vector3.one);
        }
    }
}