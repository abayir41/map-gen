using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Map.Brushes.NormalMap;
using MapGen.Placables;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.GroundBrush
{
    
    [CreateAssetMenu(fileName = "Ground Brush", menuName = "MapGen/Brushes/Ground/Ground Brush", order = 0)]
    public class GroundBrush : MultipleCellEditableBrush
    {
        [SerializeField] private GroundBrushSettings _groundBrushSettings;


        public override string BrushName => "Ground";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            CreateGround(selectedCells, grid);
        }
        
        [MethodTimer]
        private List<Placable> CreateGround(List<Vector3Int> selectedCells, Grid grid)
        {
            var result = new List<Placable>();
            foreach (var selectedCell in selectedCells)
            {
                if (grid.IsCellExist(selectedCell, out var cell)) 
                {
                    if(cell.CellState != CellState.CanBeFilled) continue;
                }

                var placable = WorldCreator.Instance.SpawnObject(selectedCell, _groundBrushSettings.Ground,
                    _groundBrushSettings.CellLayer, GroundBrushSettings.GROUND_ROTATION);
                result.Add(placable);
            }

            return result;
        }
    }
    
}