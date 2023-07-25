using System.Collections.Generic;
using MapGen.Placables.GridCreators;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.Placables
{
    public class PlacableGrid : MonoBehaviour
    {
        [SerializeField] private PlacableCellType _placableCellType;
        [SerializeField] private GridCreatorMono _gridCreator;

        public PlacableCellType PlacableCellType => _placableCellType;

        [Header("Gizmo Settings")]
        [SerializeField] public bool DrawGizmo;
        [SerializeField] private bool _gizmoRandomColor = true;
        [SerializeField] private Color _gizmoColor = Color.blue;
        [SerializeField] private float _gizmoRadius = 0.25f;


        [SerializeField] private List<Vector3Int> _cellPositions;
        public List<Vector3Int> CellPositions => _cellPositions;

        private void Awake()
        {
            DrawGizmo = false;
        }

        public void OnValidate()
        {
            if (_gizmoRandomColor)
            {
                _gizmoColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            if (_gridCreator is NewGroundGridCreator && _placableCellType != PlacableCellType.NewGround)
            {
                _placableCellType = PlacableCellType.NewGround;
                Debug.Log($"Automatic changed CellType to {_placableCellType}");
            }
            else if (_gridCreator is ShouldPlaceOnGroundGridCreator && _placableCellType != PlacableCellType.ShouldPlaceOnGround)
            {
                _placableCellType = PlacableCellType.ShouldPlaceOnGround;
                Debug.Log($"Automatic changed CellType to {_placableCellType}");
            }
            
            var gridCreator = GetComponent<GridCreatorMono>();
            if (gridCreator == null)
            {
                _gridCreator = gridCreator;
                Debug.Log("Automatically grid creator got");
            }
            
            
            _cellPositions = _gridCreator.GetGrid();
        }

        private void OnDrawGizmos()
        {
            if(!DrawGizmo) return;
            
            if(_gridCreator == null) return;

            var cellPositions = _gridCreator.GetGrid();
            foreach (var t in cellPositions)
            {
                Gizmos.color = _gizmoColor;
                var pos = transform.position + t;
                Gizmos.DrawSphere(pos, _gizmoRadius);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }

        public List<Vector3Int> GetRotatedCells(int degree)
        {
            return _cellPositions.ConvertAll(input => input.RotateVector(degree));
        }
    }
}