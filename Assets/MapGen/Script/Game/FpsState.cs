using System;
using UnityEngine;

namespace MapGen
{
    public class FpsState : State
    {
        [SerializeField] private EditState _editState;
        [SerializeField] private FpsChar _fpsController;

        public Vector3 CharSpawnPos;
        
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
            _fpsController.gameObject.SetActive(true);
            _fpsController.SetPos(CharSpawnPos);
        }

        public override void OnStateExit()
        {
            _fpsController.gameObject.SetActive(false);
        }
    }
}