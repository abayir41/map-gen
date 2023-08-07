using System;
using UnityEngine;

namespace MapGen
{
    public class FpsState : State
    {
        public static FpsState Instance
        {
            get;
            private set;
        }
        
        [SerializeField] private EditState _editState;
        [SerializeField] private FpsChar _fpsController;
        [SerializeField] private GameObject _fpsCanvas;
        
        public Vector3 CharSpawnPos;
        
        private GameManager GameManager => GameManager.Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

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
            _fpsCanvas.SetActive(true);
        }

        public override void OnStateExit()
        {
            _fpsController.gameObject.SetActive(false);
            _fpsCanvas.SetActive(false);
        }
    }
}