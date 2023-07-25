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
        
        
        
        private SelectedCellsHelper _helper;
        
        
        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            _helper = new SelectedCellsHelper(selectedCells, grid);
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

            var noise = _obstacleBrushSettings.ObjectPlacementNoise.Generate(_helper.XWidth + 1, _helper.ZWidth + 1);

            if (_obstacleBrushSettings.RotationMap is null)
            {
                foreach (var x in Enumerable.Range(_helper.MinX, _helper.XWidth).OrderBy(_ => UnityEngine.Random.value))
                foreach (var z in Enumerable.Range(_helper.MinZ, _helper.ZWidth).OrderBy(_ => UnityEngine.Random.value))
                {
                    if (ZeroOneIntervalToPercent(noise[x - _helper.MinX, z - _helper.MinZ]) < _obstacleBrushSettings.ObjectPlacementThreshold) continue;
                
                    var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();

                    var cellPos = new Vector3Int(x, layer, z);

                    foreach (var data in shuffledPlacables)
                    {
                        if (!grid.IsPlacableSuitable(cellPos, data.Placable, data.Rotation)) continue;
                    
                        WorldCreator.Instance.SpawnObject(cellPos, data.Placable, CellLayer.Obstacle, data.Rotation);
                        break;
                    }
                }    
            }
            else
            {
                foreach (var x in Enumerable.Range(_helper.MinX, _helper.XWidth).OrderBy(_ => UnityEngine.Random.value))
                foreach (var z in Enumerable.Range(_helper.MinZ, _helper.ZWidth).OrderBy(_ => UnityEngine.Random.value))
                {
                    if (ZeroOneIntervalToPercent(noise[x - _helper.MinX, z - _helper.MinZ]) < _obstacleBrushSettings.ObjectPlacementThreshold) continue;
                
                    var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();

                    var cellPos = new Vector3Int(x, layer, z);

                    foreach (var data in shuffledPlacables)
                    {
                        if (!_obstacleBrushSettings.RotationMap.IsCellExist(new Vector2Int(x, z), out var rotationMapCell)) continue;
                        if(!rotationMapCell.Rotations.Contains(data.Rotation))  continue;
                    
                        if (!grid.IsPlacableSuitable(cellPos, data.Placable, data.Rotation)) continue;
                    
                        WorldCreator.Instance.SpawnObject(cellPos, data.Placable, CellLayer.Obstacle, data.Rotation);
                        break;
                    }
                }    
            }
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
