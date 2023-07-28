using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.ObstacleSpawner
{
    
    [CreateAssetMenu(fileName = "Obstacle Brush", menuName = "MapGen/Brushes/Obstacles/Brush", order = 0)]
    public class ObstaclesBrush : Brush
    {
        [SerializeField] private ObstacleBrushSettings _obstacleBrushSettings;

        
        public ObstacleBrushSettings ObstacleBrushSettings => _obstacleBrushSettings;
        public override string BrushName => "Obstacle";
        
        public override void Paint(List<Vector3Int> selectedCells, Grid grid, Vector3Int startPoint)
        {
            var helper = new SelectedCellsHelper(selectedCells, grid);
            var layer = selectedCells.First().y;
            var rotatedPlacables = new List<PlacableData>();
            SetRandomSeed();

            
            foreach (var placable in _obstacleBrushSettings.Placables)
            {
                if (!placable.Rotatable)
                {
                    rotatedPlacables.Add(new PlacableData(placable, 0));
                    continue;
                }
                
                for (var i = 0; i < _obstacleBrushSettings.MaxObstacleRotation; i += placable.RotationDegreeStep)
                {
                    rotatedPlacables.Add(new PlacableData(placable, i));
                }
            }

            float[,] spawnProbability;
            if (_obstacleBrushSettings.UseNoiseMap)
            {
                var noise = _obstacleBrushSettings.ObjectPlacementNoise.Generate(helper.XWidth + 1, helper.ZWidth + 1);
                spawnProbability = noise;
            }
            else
            {
                spawnProbability = _obstacleBrushSettings.ObjectPlacementProbability;
            }
            
            
            foreach (var x in Enumerable.Range(helper.MinX, helper.XWidth).OrderBy(_ => UnityEngine.Random.value))
            foreach (var z in Enumerable.Range(helper.MinZ, helper.ZWidth).OrderBy(_ => UnityEngine.Random.value))
            {
                if (ZeroOneIntervalToPercent(spawnProbability[x - helper.MinX, z - helper.MinZ]) < _obstacleBrushSettings.ObjectPlacementThreshold) continue;
            
                var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();
                
                var cellPos = new Vector3Int(x, layer, z);
                
                foreach (var data in shuffledPlacables)
                {
                    if(!CanObstacleSpawnable(data, cellPos, grid)) continue;

                    WorldCreator.Instance.SpawnObject(cellPos, data.Placable, CellLayer.Obstacle, data.Rotation);
                    break;
                }
            }
        }

        private bool CanObstacleSpawnable(PlacableData data, Vector3Int cellPos, Grid grid)
        {
            var xzPos = new Vector2Int(cellPos.x, cellPos.z);

            if (_obstacleBrushSettings.RotationMap != null)
            {
                if (!_obstacleBrushSettings.RotationMap.IsCellExist(xzPos, out var rotationMapCell)) return false;
                if (!rotationMapCell.Rotations.Contains(data.Rotation))  return false;
            }
            
            if (!grid.IsPlacableSuitable(cellPos, data.Placable, data.Rotation)) return false;
            
            return true;
        }
        
        private float ZeroOneIntervalToPercent(float zeroOneInterval)
        {
            return zeroOneInterval * 100;
        }
        
        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(_obstacleBrushSettings.RandomSettings.GetSeed());
        }
    }
}
