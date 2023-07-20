using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Map.Brushes.NormalMap;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Mountains
{
    
    [CreateAssetMenu(fileName = "Mountain Brush", menuName = "MapGen/Brushes/Mountains/Brush", order = 0)]
    public class MountainBrush : ScriptableObject, IBrush
    {
        [SerializeField] private CubicPlainXYBrushArea _cubicPlainXYBrush;
        [SerializeField] private MountainBrushSettings _mountainBrushSettings;
        private SelectedCellsHelper _selectedCellsHelper;
        
        public string BrushName => "Mountain";
        public List<IBrushArea> BrushAreas => new() { _cubicPlainXYBrush };

        public void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            var yStartLevel = selectedCells.First().y;
            SetRandomSeed();
            var mountains = _mountainBrushSettings.MountainPlacementNoise.Generate(_selectedCellsHelper.XWidth + 1, _selectedCellsHelper.ZWidth + 1);

            foreach (var selectedPos in selectedCells)
            {
                var height = mountains[selectedPos.x - _selectedCellsHelper.MinX, selectedPos.z - _selectedCellsHelper.MinZ] * _mountainBrushSettings.GroundHeightFactor -
                             _mountainBrushSettings.GroundMoveDownFactor;

                for (var y = yStartLevel; y < height; y++)
                {
                    if (ZeroOneIntervalToPercent(mountains[selectedPos.x - _selectedCellsHelper.MinX, selectedPos.z - _selectedCellsHelper.MinZ]) < height) break;
                    var targetPos = new Vector3Int(selectedPos.x, y, selectedPos.z);

                    if (!grid.IsCellExist(targetPos, out var cell))
                    {
                        cell = grid.CreateCell(targetPos);
                    }
                    
                    cell.MakeCellCanBeFilledGround();
                    
                    WorldCreator.Instance.SpawnObject(targetPos, _mountainBrushSettings.Ground, CellLayer.Ground, MountainBrushSettings.GROUND_ROTATION);
                }
            }
        }
        
        private float ZeroOneIntervalToPercent(float zeroOneInterval)
        {
            return zeroOneInterval * 100;
        }
        
        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(_mountainBrushSettings.RandomSettings.GetSeed());
        }
    }
}