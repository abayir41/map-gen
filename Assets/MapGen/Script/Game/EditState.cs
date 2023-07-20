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
        [SerializeField] private TextMeshProUGUI yOffsetText;
        
        
        private GameManager GameManager => GameManager.Instance;
        private WorldEdit WorldEdit => WorldEdit.Instance;
        private BrushSelector BrushSelector => BrushSelector.Instance;

        private void Start()
        {
            yOffsetText.text = "Brush Y Offset: " + WorldEdit.SelectedAreYOffset;
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
                }
                else
                {
                    WorldEdit.SelectedAreYOffset++;
                    yOffsetText.text = "Brush Y Offset: " + WorldEdit.SelectedAreYOffset;
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
                }
                else
                {
                    WorldEdit.SelectedAreYOffset--;
                    yOffsetText.text = "Brush Y Offset: " + WorldEdit.SelectedAreYOffset;
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameManager.SwitchState(this, _fpsState);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                BrushSelector.PreviousBrush();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                BrushSelector.NextBrush();
            }
        }

        public override void OnStateEnter()
        {
            GizmoToggle.ToggleGizmos(true);
            _camera.SetActive(true);
            _worldCreator.SetActive(true);
            _gridSelector.SetActive(true);
        }

        public override void OnStateExit()
        {
            GizmoToggle.ToggleGizmos(false);
            _camera.SetActive(false);
            _gridSelector.SetActive(false);
            _worldCreator.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(_fpsState.CharSpawnPos, Vector3.one);
        }
    }
}