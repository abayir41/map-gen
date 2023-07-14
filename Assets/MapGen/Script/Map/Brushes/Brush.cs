using System.Collections.Generic;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class Brush : ScriptableObject
    {
        public abstract List<Placable> Paint(List<Vector3Int> selectedCells, Grid grid);
    }
}