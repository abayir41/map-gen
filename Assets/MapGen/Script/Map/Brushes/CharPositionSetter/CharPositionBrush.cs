using System.Collections.Generic;
using System.Linq;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    [CreateAssetMenu(fileName = "Char Position Setter", menuName = "MapGen/Brushes/Char Position Setter", order = 0)]
    public class CharPositionBrush : ScriptableObject, IBrush
    {
        public string BrushName => "Char Pos Setter";
        public List<IBrushArea> BrushAreas => new() { _charPositionSetterArea };
        
        
        [SerializeField] private CharPositionSetterArea _charPositionSetterArea;
        

        public void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            FpsState.Instance.CharSpawnPos = selectedCells.First();
        }
    }
}