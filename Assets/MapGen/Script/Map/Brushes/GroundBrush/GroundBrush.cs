using System.Collections.Generic;
using MapGen.Command;
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
        private const int GROUND_ROTATION = 0;
        [SerializeField] private Placable _ground;
        [SerializeField] private CellLayer _cellLayer = CellLayer.Ground;
        
        public override string BrushName => "Ground";
        protected override int HitBrushHeight => 1;

        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new GroundBrushCommand(WorldCreator.Instance, this, new List<Vector3Int>(selectedCells), grid);
        }
        
        public List<SpawnData> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var data = new List<SpawnData>();
            
            foreach (var selectedCell in selectedCells)
            {
                if (grid.IsCellExist(selectedCell, out var cell)) 
                {
                    if(cell.CellState != CellState.CanBeFilled) continue;
                }

                var groundData = new SpawnData(selectedCell, _ground, GROUND_ROTATION, _cellLayer);
                WorldCreator.Instance.SpawnObject(groundData);
                data.Add(groundData);
            }

            return data;
        }
    }
    
}