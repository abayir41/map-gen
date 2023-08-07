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
        [SerializeField] private float _gizmoRadius = 0.25f;
        [SerializeField] private Color _visualCellsColor = Color.green;
        [SerializeField] private Sprite _brushIcon;

        public Sprite BrushIcon => _brushIcon;
        public abstract string BrushName { get; }
        protected abstract int HitBrushHeight { get; }
        protected List<Vector3Int> VisualCells { get; set; } = new();
        protected WorldCreator WorldCreator => WorldCreator.Instance;
        protected EditState EditState => EditState.Instance;
        protected bool DidRayHit { get; private set; }
        protected Vector3Int HitPosOffsetted { get; private set; }
        protected Vector3Int HitPos { get; private set; }

        private void OnEnable()
        {
            VisualCells = new();
        }

        public virtual void Update()
        {
            if (EditState.RayToGridCell(out var cellPos))
            {
                DidRayHit = true;
                HitPos = cellPos;
                HitPosOffsetted = cellPos + Vector3Int.up * (EditState.SelectedAreYOffset + HitBrushHeight);
            }
            else
            {
                DidRayHit = false;
                VisualCells.Clear();
            }
        }

        public virtual void OnDrawGizmos()
        {
            Gizmos.color = _visualCellsColor;
            foreach (var visualCell in VisualCells)
            {
                var pos = WorldCreator.Grid.CellPositionToRealWorld(visualCell);
                Gizmos.DrawSphere(pos, _gizmoRadius);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
        
    }
}