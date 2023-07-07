using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {

        [Header("Placable Properties")] 
        [SerializeField] private bool _rotatable;
        [Range(1,359)] [SerializeField] private int _rotationDegreeStep = 15;
        public int RotationDegreeStep => Mathf.Clamp(_rotationDegreeStep,1,360);
        
        [Header("Grid Properties")]
        [SerializeField] protected List<Vector3Int> requiredGrids;
        [SerializeField] protected List<Vector3Int> lockGrids;
        [SerializeField] protected List<Vector3Int> shouldPlacedOnGroundGrids;
        [SerializeField] protected List<Vector3Int> newGroundGrids;
        [SerializeField] protected bool useVisualsAsRequiredGrids;
        [SerializeField] protected bool useVisualAsShouldPlacedOnGroundGrids;
        [SerializeField] protected bool useVisualsAsNewGroundGrids;
        [SerializeField] protected bool useCubeRequiredGridStyle;
        [SerializeField] protected Transform visualsParent;
        [SerializeField] private Vector2Int cubeLockOffsetX;
        [SerializeField] private Vector2Int cubeLockOffsetY;
        [SerializeField] private Vector2Int cubeLockOffsetZ;

        public bool Rotatable => _rotatable;
        public List<Vector3Int> RequiredGrids => requiredGrids;
        public List<Vector3Int> ShouldPlacedOnGroundGrids => shouldPlacedOnGroundGrids;
        public List<Vector3Int> NewGroundGrids => newGroundGrids;
        public List<Vector3Int> LockGrids => lockGrids;
        public Transform VisualsParent => visualsParent;

        [Header("Gizmo Settings")]
        [SerializeField] protected float gizmoRadius = 0.25f;
        [SerializeField] protected float offsetScaler = 1f;
        [SerializeField] protected PlacableGizmos _placableGizmos;

        private void Awake()
        {
            _placableGizmos = PlacableGizmos.None;
        }

        protected virtual void OnValidate()
        {
            if (useVisualsAsRequiredGrids)
            {
                requiredGrids = new List<Vector3Int>();
                foreach(Transform child in visualsParent)
                    requiredGrids.Add(Vector3Int.FloorToInt(child.localPosition));
            }

            if (useVisualAsShouldPlacedOnGroundGrids)
            {
                shouldPlacedOnGroundGrids = new List<Vector3Int>();
                foreach(Transform child in visualsParent)
                    if(child.localPosition.y == 0)
                        shouldPlacedOnGroundGrids.Add(Vector3Int.FloorToInt(child.localPosition));
            }

            if (useVisualsAsNewGroundGrids)
            {
                newGroundGrids = new List<Vector3Int>();
                foreach(Transform child in visualsParent)
                    newGroundGrids.Add(Vector3Int.FloorToInt(child.localPosition) + Vector3Int.up);
            }

            if (useCubeRequiredGridStyle)
            {
                requiredGrids = new List<Vector3Int>();

                for(var x = cubeLockOffsetX.x + 1; x < cubeLockOffsetX.y; x++)
                for(var y = cubeLockOffsetY.x + 1; y < cubeLockOffsetY.y; y++)
                for (var z = cubeLockOffsetZ.x + 1; z < cubeLockOffsetZ.y; z++)
                {
                    requiredGrids.Add(new Vector3Int(x, y, z));
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if(requiredGrids != null && _placableGizmos.HasFlag(PlacableGizmos.RequiredGrids))
                foreach (var t in requiredGrids)
                {
                    Gizmos.color = Color.blue;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }

            if(lockGrids != null && _placableGizmos.HasFlag(PlacableGizmos.LockGrids))
                foreach (var t in lockGrids)
                {
                    Gizmos.color = Color.red;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
            
            if(shouldPlacedOnGroundGrids != null && _placableGizmos.HasFlag(PlacableGizmos.ShouldPlaceOnGround))
                foreach (var t in shouldPlacedOnGroundGrids)
                {
                    Gizmos.color = Color.black;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
            
            if(newGroundGrids != null && _placableGizmos.HasFlag(PlacableGizmos.NewGround))
                foreach (var t in newGroundGrids)
                {
                    Gizmos.color = Color.yellow;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
        }
    }
}