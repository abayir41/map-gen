using System;
using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class Brush : ScriptableObject
    {
        [SerializeField] private Sprite _brushIcon;
        [SerializeField] private LayerMask _acceptedLayerMask;
        
        public Sprite BrushIcon => _brushIcon;
        public abstract string BrushName { get; }
        protected abstract int HitBrushHeight { get; }
        protected WorldCreator WorldCreator => WorldCreator.Instance;
        protected EditState EditState => EditState.Instance;
        protected bool DidRayHit { get; private set; }
        protected Vector3Int HitPosOffsetted { get; private set; }
        protected Vector3Int HitPos { get; private set; }
        

        public virtual void Update()
        {
            if (EditState.RayToGridCell(out var cellPos, _acceptedLayerMask))
            {
                DidRayHit = true;
                HitPos = cellPos;
                HitPosOffsetted = cellPos + Vector3Int.up * (EditState.SelectedAreYOffset + HitBrushHeight);
            }
            else
            {
                DidRayHit = false;
            }
        }

        public virtual void OnDrawGizmos()
        {
            
        }
    }
}