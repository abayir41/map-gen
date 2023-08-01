using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Map.Brushes.NormalMap;
using MapGen.Placables;
using MapGen.Random;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Mountains
{
    
    [CreateAssetMenu(fileName = "Mountain Brush", menuName = "MapGen/Brushes/Mountains/Brush", order = 0)]
    public class MountainBrush : MultipleCellEditableBrush
    {
        public const int GROUND_ROTATION = 0;

        [SerializeField] private RandomSettings randomSettings;
        [SerializeField] private Placable ground;
        [SerializeField] private Noise.Noise _mountainPlacementNoise;
        [SerializeField] private float groundHeightFactor;
        [SerializeField] private float groundMoveDownFactor;

        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise MountainPlacementNoise => _mountainPlacementNoise;
        public float GroundHeightFactor => groundHeightFactor;
        public float GroundMoveDownFactor => groundMoveDownFactor;
        public Placable Ground => ground;
        
        
        private SelectedCellsHelper _selectedCellsHelper;
        public override string BrushName => "Mountain";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            var yStartLevel = selectedCells.First().y;
            SetRandomSeed();
            var mountains = MountainPlacementNoise.Generate(_selectedCellsHelper.XWidth + 1, _selectedCellsHelper.ZWidth + 1);

            foreach (var selectedPos in selectedCells)
            {
                var height = mountains[selectedPos.x - _selectedCellsHelper.MinX, selectedPos.z - _selectedCellsHelper.MinZ] * GroundHeightFactor -
                             GroundMoveDownFactor;

                for (var y = yStartLevel; y < height; y++)
                {
                    if (ZeroOneIntervalToPercent(mountains[selectedPos.x - _selectedCellsHelper.MinX, selectedPos.z - _selectedCellsHelper.MinZ]) < height) break;
                    var targetPos = new Vector3Int(selectedPos.x, y, selectedPos.z);

                    if (grid.IsCellExist(targetPos, out var cell))
                    {
                        if(cell.CellState != CellState.CanBeFilled) continue;
                    }

                    WorldCreator.Instance.SpawnObject(targetPos, Ground, CellLayer.Ground, GROUND_ROTATION);
                }
            }
        }
        
        private float ZeroOneIntervalToPercent(float zeroOneInterval)
        {
            return zeroOneInterval * 100;
        }
        
        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(RandomSettings.GetSeed());
        }
    }
}