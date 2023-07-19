using System;
using UnityEngine;

namespace MapGen
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private EditState _editState;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _editState.OnStateEnter();
            _editState.gameObject.SetActive(true);
        }

        public void SwitchState(State oldState, State newState)
        {
            oldState.OnStateExit();
            oldState.gameObject.SetActive(false);

            newState.gameObject.SetActive(true);
            newState.OnStateEnter();
        }
    }
}