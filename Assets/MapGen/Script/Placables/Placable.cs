using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        [Header("Grid Properties")]
        [SerializeField] private List<Vector3Int> requiredGrids;
        [SerializeField] private List<Vector3Int> lockGrids;
        [SerializeField] private List<Vector3Int> shouldPlacedOnGroundGrids;
        [SerializeField] private List<Vector3Int> newGroundGrids;
        [SerializeField] private bool useVisualsAsRequiredGrids;
        [SerializeField] private bool useVisualAsShouldPlacedOnGroundGrids;
        [SerializeField] private Transform visualsParent;
        [SerializeField] private bool useCubeLockStyle;
        [SerializeField] private int cubeLockOffsetX;
        [SerializeField] private int cubeLockOffsetY;
        [SerializeField] private int cubeLockOffsetZ;

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

        private void OnValidate()
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

            if (useCubeLockStyle)
            {
                lockGrids = new List<Vector3Int>();
                
                var minX = requiredGrids.Min(i => i.x);
                var minY = requiredGrids.Min(i => i.y);
                var minZ = requiredGrids.Min(i => i.z);

                var maxX = requiredGrids.Max(i => i.x);
                var maxY = requiredGrids.Max(i => i.y);
                var maxZ = requiredGrids.Max(i => i.z);

                for(var x = minX - cubeLockOffsetX + 1; x < maxX + cubeLockOffsetX; x++)
                for(var y = minY - cubeLockOffsetY + 1; y < maxY + cubeLockOffsetY; y++)
                for (var z = minZ - cubeLockOffsetZ + 1; z < maxZ + cubeLockOffsetZ; z++)
                {
                    lockGrids.Add(new Vector3Int(x, y, z));
                }
            }
        }

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