using System;
using System.Collections.Generic;
using MapGen.GridSystem;
using UnityEngine;
using Weaver;

namespace MapGen.Map.MapEdit.Brushes
{
    
    public abstract class BrushSettings : ScriptableObject
    {
        [SerializeField] private LayerMask _targetSelectableGridCells;
        [SerializeField] protected Vector2Int _brushSizeX; 
        [SerializeField] protected Vector2Int _brushSizeY;
        [SerializeField] protected Vector2Int _brushSizeZ; 

        public LayerMask TargetSelectableGridCells => _targetSelectableGridCells;
        public abstract Vector2Int BrushSizeX { get; }
        public abstract Vector2Int BrushSizeY { get; }
        public abstract Vector2Int BrushSizeZ { get; }
        
        public abstract List<GridCell> GetWantedAreaAsGridCell(List<GridCell> selectedCells, Grid grid);
        public abstract List<Vector3Int> GetWantedAreaAsV3Int(List<GridCell> selectedCells, Grid grid);

    }
}