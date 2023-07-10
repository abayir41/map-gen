using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public abstract class PlacableGrid : MonoBehaviour
    {
        [Header("Gizmo Settings")]
        [SerializeField] public bool DrawGizmo;
        [SerializeField] private bool _gizmoRandomColor = true;
        [SerializeField] private Color _gizmoColor = Color.blue;
        [SerializeField] private float _gizmoRadius = 0.25f;
        
        [Header("Method 1")]
        [SerializeField] protected bool _useVisuals;
        [SerializeField] protected Transform _visualsParent;
        
        [Header("Method 2")]
        [SerializeField] protected bool _useCubicGridStyle;
        [SerializeField] protected Vector2Int _cubeLockOffsetX = Vector2Int.up * 5;
        [SerializeField] protected Vector2Int _cubeLockOffsetY = Vector2Int.up * 5;
        [SerializeField] protected Vector2Int _cubeLockOffsetZ = Vector2Int.up * 5;
        
        
        [SerializeField] protected List<Vector3Int> _cellPositions;

        public List<Vector3Int> CellPositions => _cellPositions;
        
        public abstract PlacableCellType GetCellType();

        private void Awake()
        {
            DrawGizmo = false;
        }

        public void OnValidate()
        {
            _visualsParent = transform.root.Find("Visuals");
            
            if (_gizmoRandomColor)
            {
                _gizmoColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            if (_useVisuals && _useCubicGridStyle)
            {
                Debug.LogWarning("Both Use Visuals and Use Cubic Grid Style are enabled. Use Cubic Grid Style will be used");
            }

            if (_useVisuals)
            {
                VisualAlgorithm();
            }

            if (_useCubicGridStyle)
            {
                CubicGridStyleAlgorithm();
            }
        }

        protected virtual void VisualAlgorithm()
        {
            _cellPositions = new List<Vector3Int>();
            foreach(Transform child in _visualsParent)
                _cellPositions.Add(Vector3Int.FloorToInt(child.localPosition));
        }
        
        protected virtual void CubicGridStyleAlgorithm()
        {
            _cellPositions = new List<Vector3Int>();

            for(var x = _cubeLockOffsetX.x + 1; x < _cubeLockOffsetX.y; x++)
            for(var y = _cubeLockOffsetY.x + 1; y < _cubeLockOffsetY.y; y++)
            for (var z = _cubeLockOffsetZ.x + 1; z < _cubeLockOffsetZ.y; z++)
            {
                _cellPositions.Add(new Vector3Int(x, y, z));
            }
        }
        
        private void OnDrawGizmos()
        {
            if(!DrawGizmo) return; ;
            
            if(_cellPositions == null) return;
            
            foreach (var t in _cellPositions)
            {
                Gizmos.color = _gizmoColor;
                var pos = transform.position + (Vector3)t;
                Gizmos.DrawSphere(pos, _gizmoRadius);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
    }
}