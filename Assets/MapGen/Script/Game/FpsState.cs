using System;
using UnityEngine;

namespace MapGen
{
    public class FpsState : State
    {
        [SerializeField] private EditState _editState;
        [SerializeField] private GameObject _fpsController;

        public Vector3 charSpawnPos;
        
        private GameManager GameManager => GameManager.Instance;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameManager.SwitchState(this, _editState);
            }
        }

        public override void OnStateEnter()
        {
            _fpsController.transform.position = charSpawnPos;
            _fpsController.SetActive(true);
        }

        public override void OnStateExit()
        {
            _fpsController.SetActive(false);
        }
    }
}