using System.Collections.Generic;
using MapGen.Map.Brushes.ObstacleSpawner;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class ObstaclesCommand : MultipleCellBrushCommandBase<ObstaclesBrush>
    {
        private readonly int _seed;
        
        public ObstaclesCommand(WorldCreator worldCreator, ObstaclesBrush brush, List<Vector3Int> selectedCells, Grid grid, int seed) : base(worldCreator, brush, selectedCells, grid)
        {
            _seed = seed;
        }

        public override void Execute()
        {
            _data = _brush.Paint(_selectedCells, _grid, _seed);
        }
    }
}