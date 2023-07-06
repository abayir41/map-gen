using System.Collections.Generic;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.TunnelSystem
{
    public class TunnelBrush : Placable
    {
        [SerializeField] private List<Vector3Int> destroyPoints;

        public List<Vector3Int> DestroyPoints => destroyPoints;

        [SerializeField] private bool drawDestroyPoints;
        [SerializeField] private bool useCubeDestroyPointGridStyle;
        [SerializeField] private Vector2Int cubeDestroyOffsetX;
        [SerializeField] private Vector2Int cubeDestroyOffsetY;
        [SerializeField] private Vector2Int cubeDestroyOffsetZ;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (useCubeDestroyPointGridStyle)
            {
                destroyPoints = new List<Vector3Int>();

                for(var x = cubeDestroyOffsetX.x + 1; x < cubeDestroyOffsetX.y; x++)
                for(var y = cubeDestroyOffsetY.x + 1; y < cubeDestroyOffsetY.y; y++)
                for (var z = cubeDestroyOffsetZ.x + 1; z < cubeDestroyOffsetZ.y; z++)
                {
                    destroyPoints.Add(new Vector3Int(x, y, z));
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if(!drawGizmo) return;

            if(destroyPoints != null && drawDestroyPoints)
                foreach (var t in destroyPoints)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(transform.position + (Vector3) t * offsetScaler, gizmoRadius);
                }
            
            
        }
    }
}