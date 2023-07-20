using System.Collections.Generic;
using System.Linq;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class CharPositionBrush : IBrush
    {
        public string BrushName => "Char Pos Setter";

        private FpsState _fpsState;
        public CharPositionBrush(FpsState fpsState)
        {
            _fpsState = fpsState;
        }

        public void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            _fpsState.CharSpawnPos = selectedCells.First();
        }
    }
}