using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map.Brushes
{
    public abstract class Brush : ScriptableObject
    {
        public abstract List<Placable> Paint(List<GridCell> selectedCells, Grid grid);
    }
}