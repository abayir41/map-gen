using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        [SerializeField] private List<Vector3Int> requiredGrids;
        [SerializeField] private List<Vector3Int> lockGrids;
        [SerializeField] private List<Vector3Int> shouldPlacedOnGroundGrids;
        [SerializeField] private List<Vector3Int> newGroundGrids;

        public List<Vector3Int> RequiredGrids => requiredGrids;
        public List<Vector3Int> ShouldPlacedOnGroundGrids => shouldPlacedOnGroundGrids;
        public List<Vector3Int> NewGroundGrids => newGroundGrids;
        public List<Vector3Int> LockGrids => lockGrids;

        [Header("Gizmo Settings")]
        [SerializeField] private bool drawGizmo = true;
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private float offsetScaler = 1f;
        [SerializeField] private bool drawRequireGrids;
        [SerializeField] private bool drawLockGrids;
        [SerializeField] private bool drawShouldPlaceGroundGrids;
        [SerializeField] private bool drawNewGroundGrids;
        
        private void OnDrawGizmos()
        {
            if(!drawGizmo) return;

            if(requiredGrids != null && drawRequireGrids)
                foreach (var t in requiredGrids)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }

            if(lockGrids != null && drawLockGrids)
                foreach (var t in lockGrids)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
            
            if(shouldPlacedOnGroundGrids != null && drawShouldPlaceGroundGrids)
                foreach (var t in shouldPlacedOnGroundGrids)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
            
            if(newGroundGrids != null && drawNewGroundGrids)
                foreach (var t in newGroundGrids)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
        }
    }
}