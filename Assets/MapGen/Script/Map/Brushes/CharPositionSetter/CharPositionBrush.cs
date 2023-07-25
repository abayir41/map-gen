using System.Collections.Generic;
using System.Linq;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    [CreateAssetMenu(fileName = "Char Position Setter", menuName = "MapGen/Brushes/Char Position Setter", order = 0)]
    public class CharPositionBrush : Brush
    {
        [SerializeField] private CharPositionSetterArea _charPositionSetterArea;

        public override List<BrushArea> BrushAreas => new List<BrushArea>() { _charPositionSetterArea };


        public override string BrushName => "Char Pos Setter";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            FpsState.Instance.CharSpawnPos = selectedCells.First();
        }
    }
}