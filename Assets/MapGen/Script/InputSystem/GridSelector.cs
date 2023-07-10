using System;
using System.Collections.Generic;
using MapGen.GridSystem;
using Maze;
using UnityEngine;

namespace MapGen.InputSystem
{
    public class GridSelector : MonoBehaviour
    {
        private static InputManager InputManager => InputManager.Instance;
        private Vector3Int _lastCellPos;
        private List<Vector3Int> _selectedPoss = new();

        private void Update()
        {
            if (InputManager.GetSelectedMapPosition(out var hit))
            {
                var gridCell = hit.collider.GetComponent<GridCellMono>();
                _lastCellPos = gridCell.GridPos;

                if (InputManager.IsGridSelected())
                {
                    if (!_selectedPoss.Contains(_lastCellPos))
                    {
                        _selectedPoss.Add(_lastCellPos);
                    }
                }
            };
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_lastCellPos, Vector3.one);
            
            Gizmos.color = Color.green;
            foreach (var cellPos in _selectedPoss)
            {
                Gizmos.DrawWireCube(cellPos, Vector3.one);
            }
        }
    }
}