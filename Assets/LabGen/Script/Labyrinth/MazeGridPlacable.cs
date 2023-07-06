using System;
using UnityEngine;

namespace LabGen.Labyrinth
{
    public class MazeGridPlacable : MonoBehaviour
    {
        [SerializeField] private GameObject _leftWall;
        [SerializeField] private GameObject _rightWall;
        [SerializeField] private GameObject _topWall;
        [SerializeField] private GameObject _bottomWall;

        private void Awake()
        {
            _leftWall.SetActive(false);
            _rightWall.SetActive(false);
            _topWall.SetActive(false);
            _bottomWall.SetActive(false);
        }

        public void OpenLeftWall()
        {
            _leftWall.SetActive(true);
        }

        public void OpenRightWall()
        {
            _rightWall.SetActive(true);
        }

        public void OpenTopWall()
        {
            _topWall.SetActive(true);
        }

        public void OpenBottomWall()
        {
            _bottomWall.SetActive(true);
        }
    }
}