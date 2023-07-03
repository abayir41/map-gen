using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        [SerializeField] private List<Vector3Int> requiredGrids;
        [SerializeField] private List<Vector3Int> unlockGrids;
        [SerializeField] private List<Vector3Int> lockGrids;

        public List<Vector3Int> RequiredGrids => requiredGrids;
        public List<Vector3Int> UnlockGrids => unlockGrids;
        public List<Vector3Int> LockGrids => lockGrids;

        [Header("Gizmo Settings")]
        [SerializeField] private bool drawGizmo = true;
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private float offsetScaler = 1f;

        public void Destroy()
        {
            Destroy(gameObject);
        }
        private void OnDrawGizmos()
        {
            if(!drawGizmo) return;

            if(requiredGrids != null)
                foreach (var t in requiredGrids)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
            
            if(unlockGrids != null)
                foreach (var t in unlockGrids)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
            
            if(lockGrids != null)
                foreach (var t in lockGrids)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
        }
    }
}