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
        [SerializeField] private bool useCubeRequiredGridStyle;
        [SerializeField] private Vector2Int cubeLockOffsetX;
        [SerializeField] private Vector2Int cubeLockOffsetY;
        [SerializeField] private Vector2Int cubeLockOffsetZ;

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

        private void Awake()
        {
            drawGizmo = false;
        }

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