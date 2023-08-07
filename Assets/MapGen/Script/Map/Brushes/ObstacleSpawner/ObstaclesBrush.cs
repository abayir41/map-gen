using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using MapGen.Random;
using MapGen.Utilities;
using UnityEditor;
using UnityEngine;
using Weaver;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.ObstacleSpawner
{
    
    [CreateAssetMenu(fileName = "Obstacle Brush", menuName = "MapGen/Brushes/Obstacles/Brush", order = 0)]
    public class ObstaclesBrush : MultipleCellEditableBrush
    {
        [Header("Object Placement Probability Settings")]
        [SerializeField] private Noise.Noise objectPlacementNoise;
        [SerializeField] private RandomSettings randomSettings;
        [SerializeField] private bool _useNoiseMap;
        public float[,] ObjectPlacementProbability { get; set; }
        public RotationMap RotationMap { get; set; }
        
        [Header("Obstacles")]
        [SerializeField] private List<Placable> placables;
        [SerializeField] private float _maxObstacleRotation;
        [SerializeField] [Range(0,100)] private float objectPlacementThreshold;
        
        
        public List<Placable> Placables => placables;
        public float MaxObstacleRotation => _maxObstacleRotation;
        public RandomSettings RandomSettings => randomSettings;
        public Noise.Noise ObjectPlacementNoise => objectPlacementNoise;
        public float ObjectPlacementThreshold => objectPlacementThreshold;

        public bool UseNoiseMap
        {
            get => _useNoiseMap;
            set => _useNoiseMap = value;
        }
        
        public override string BrushName => "Obstacle Forest";
        protected override int HitBrushHeight => 1;

        private struct PlacableData
        {
            public Placable Placable { get; }
            public int Rotation { get; }
        
            public PlacableData(Placable placable, int rotation)
            {
                Placable = placable;
                Rotation = rotation;
            }
        }
        
        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new MultipleCellEditCommand(WorldCreator.Instance,this, selectedCells, grid);
        }

        public override List<SpawnData> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var result = new List<SpawnData>();
            var helper = new SelectedCellsHelper(selectedCells, grid);
            var layer = selectedCells.First().y;
            var rotatedPlacables = new List<PlacableData>();
            SetRandomSeed();

            
            foreach (var placable in Placables)
            {
                if (!placable.Rotatable)
                {
                    rotatedPlacables.Add(new PlacableData(placable, 0));
                    continue;
                }
                
                for (var i = 0; i < MaxObstacleRotation; i += placable.RotationDegreeStep)
                {
                    rotatedPlacables.Add(new PlacableData(placable, i));
                }
            }

            float[,] spawnProbability;
            if (UseNoiseMap)
            {
                var noise = ObjectPlacementNoise.Generate(helper.XWidth + 1, helper.ZWidth + 1, RandomSettings.GetSeed());
                spawnProbability = noise;
            }
            else
            {
                spawnProbability = ObjectPlacementProbability;
            }
            
            
            foreach (var x in Enumerable.Range(helper.MinX, helper.XWidth).OrderBy(_ => UnityEngine.Random.value))
            foreach (var z in Enumerable.Range(helper.MinZ, helper.ZWidth).OrderBy(_ => UnityEngine.Random.value))
            {
                if (ZeroOneIntervalToPercent(spawnProbability[x - helper.MinX, z - helper.MinZ]) < ObjectPlacementThreshold) continue;

                var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();
                var cellPos = new Vector3Int(x, layer, z);

                foreach (var data in shuffledPlacables)
                {
                    if(!CanObstacleSpawnable(data, cellPos, grid)) continue;

                    var spawnData = new SpawnData(cellPos, data.Placable, data.Rotation, CellLayer.Obstacle);
                    WorldCreator.Instance.SpawnObject(spawnData);
                    result.Add(spawnData);
                    break;
                }
            }

            return result;
        }

        private bool CanObstacleSpawnable(PlacableData data, Vector3Int cellPos, Grid grid)
        {
            var xzPos = new Vector2Int(cellPos.x, cellPos.z);

            if (RotationMap != null)
            {
                if (!RotationMap.IsCellExist(xzPos, out var rotationMapCell)) return false;
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
            UnityEngine.Random.InitState(RandomSettings.GetSeed());
        }
    }
}
