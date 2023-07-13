using System;
using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.GridSystem.Obsolete;
using UnityEngine;
using Weaver;
using GridCell = MapGen.GridSystem.Obsolete.GridCell;

namespace MapGen.Map.MapEdit.Brushes
{
    
    [CreateAssetMenu(fileName = "Cubic Brush Settings", menuName = "MapGen/Map Edit/Brushes/Cubic Brush Settings", order = 0)]
    public class CubicBrushSettings : ScriptableObject
    {
        [SerializeField] private LayerMask _targetSelectableGridCells;
        [SerializeField] private Vector3Int _brushSize;

        public LayerMask TargetSelectableGridCells => _targetSelectableGridCells;
        public Vector3Int BrushSize => _brushSize;
    }
}