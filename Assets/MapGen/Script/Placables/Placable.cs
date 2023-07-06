using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        
        [Header("Placable Properties")] 
        
        [Range(1,359)]
        [SerializeField] 
        private int rotationDegreeStep = 15;
        public int RotationDegreeStep => Mathf.Clamp(rotationDegreeStep,1,360);
        
        [Header("Grid Properties")]
        [SerializeField] private List<Vector3Int> requiredGrids;
        [SerializeField] private List<Vector3Int> lockGrids;
        [SerializeField] private List<Vector3Int> shouldPlacedOnGroundGrids;
        [SerializeField] private List<Vector3Int> newGroundGrids;
        [SerializeField] private bool useVisualsAsRequiredGrids;
        [SerializeField] private bool useVisualAsShouldPlacedOnGroundGrids;
        [SerializeField] private bool useVisualsAsNewGroundGrids;
        [SerializeField] private bool useCubeRequiredGridStyle;
        [SerializeField] private Transform visualsParent;
        [SerializeField] private Vector2Int cubeLockOffsetX;
        [SerializeField] private Vector2Int cubeLockOffsetY;
        [SerializeField] private Vector2Int cubeLockOffsetZ;

        public List<Vector3Int> RequiredGrids => requiredGrids;
        public List<Vector3Int> ShouldPlacedOnGroundGrids => shouldPlacedOnGroundGrids;
        public List<Vector3Int> NewGroundGrids => newGroundGrids;
        public List<Vector3Int> LockGrids => lockGrids;
        public Transform VisualsParent => visualsParent;

        [Header("Gizmo Settings")]
        [SerializeField] protected bool drawGizmo = true;
        [SerializeField] protected float gizmoRadius = 0.25f;
        [SerializeField] protected float offsetScaler = 1f;
        [SerializeField] private bool drawRequireGrids;
        [SerializeField] private bool drawLockGrids;
        [SerializeField] private bool drawShouldPlaceGroundGrids;
        [SerializeField] private bool drawNewGroundGrids;

        protected virtual void Awake()
        {
            drawGizmo = false;
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
            if(!drawGizmo) return;

            if(requiredGrids != null && drawRequireGrids)
                foreach (var t in requiredGrids)
                {
                    Gizmos.color = Color.blue;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }

            if(lockGrids != null && drawLockGrids)
                foreach (var t in lockGrids)
                {
                    Gizmos.color = Color.red;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
            
            if(shouldPlacedOnGroundGrids != null && drawShouldPlaceGroundGrids)
                foreach (var t in shouldPlacedOnGroundGrids)
                {
                    Gizmos.color = Color.black;
                    var pos = transform.position + (Vector3)t * offsetScaler;
                    Gizmos.DrawSphere(pos, gizmoRadius);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
            
            if(newGroundGrids != null && drawNewGroundGrids)
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