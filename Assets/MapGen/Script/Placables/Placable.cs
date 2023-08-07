using System;
using System.Collections.Generic;
using System.Linq;
using MapGen.Map;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        
        [Header("Placable Properties")] 
        [SerializeField] private bool _rotatable = true;
        [Range(1,359)] [SerializeField] private int _rotationDegreeStep = 15;
        [SerializeField] private Vector3Int _origin;
        
        [Header("Grid Properties")] 
        [SerializeField] protected List<PlacableGrid> _grids;
        [SerializeField] protected Transform _visualsParent;
        
        [Header("Gizmo Settings")]
        [SerializeField] public bool DrawGizmo;
        [SerializeField] private Color _gizmoColor = Color.red;
        [SerializeField] private float _gizmoRadius = 0.25f;
        
        public SpawnData SpawnData { get; private set; }
        public int RotationDegreeStep => Mathf.Clamp(_rotationDegreeStep,1,360);
        public List<PlacableGrid> Grids => _grids;
        public bool Rotatable => _rotatable;
        public Vector3Int Origin => _origin;

        public virtual void InitializePlacable(SpawnData spawnData)
        {
            SpawnData = spawnData;
        }

        public void OnValidate()
        {
            _grids = GetComponentsInChildren<PlacableGrid>().ToList();
            
            var possibleVisualParent = transform.root.Find("Visuals");
            if (_visualsParent != possibleVisualParent)
            {
                _visualsParent = possibleVisualParent;
                Debug.Log("Automatically visual parent added");
            }
            
            if (_visualsParent.localPosition.ToVector3Int() != _origin)
            {
                _visualsParent.localPosition = _origin;
                Debug.Log("Visuals parent set to grid origin.");
            }
        }

        private void OnDrawGizmos()
        {
            if(!DrawGizmo) return;
            
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(_origin, _gizmoRadius);
            Gizmos.DrawWireCube(_origin, Vector3.one);
        }

        public void Rotate(float degree)
        {
            _visualsParent.eulerAngles = Vector3.up * degree;
        }
    }
}