﻿using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
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
    public class MountainBrush : MultipleCellEditableBrush, IRandomBrush
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
        
        
        private SelectedCellsHelper _selectedCellsHelper;
        public override string BrushName => "Mountain";
        protected override int HitBrushHeight => 1;

        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new MountainsCommand(WorldCreator.Instance,this, new List<Vector3Int>(selectedCells), grid, randomSettings.GetSeed());
        }
        
        public ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid, int seed)
        {
            return new MountainsCommand(WorldCreator.Instance,this, new List<Vector3Int>(selectedCells), grid, seed);
        }

        public List<SpawnData> Paint(List<Vector3Int> selectedCells, Grid grid, int seed)
        {
            var result = new List<SpawnData>();
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            var yStartLevel = selectedCells.First().y;
            SetRandomSeed(seed);
            var mountains = MountainPlacementNoise.Generate(_selectedCellsHelper.XWidth + 1, _selectedCellsHelper.ZWidth + 1, seed);

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

                    var groundData = new SpawnData(targetPos, ground, GROUND_ROTATION, CellLayer.Ground);
                    WorldCreator.Instance.SpawnObject(groundData);
                    result.Add(groundData);
                }
            }

            return result;
        }

        private float ZeroOneIntervalToPercent(float zeroOneInterval)
        {
            return zeroOneInterval * 100;
        }
        
        private void SetRandomSeed(int seed)
        {
            UnityEngine.Random.InitState(seed);
        }
    }
}