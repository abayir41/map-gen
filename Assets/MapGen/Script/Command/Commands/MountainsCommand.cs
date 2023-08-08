using System.Collections.Generic;
using MapGen.Map.Brushes.Mountains;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class MountainsCommand : MultipleCellBrushCommandBase<MountainBrush>
    {
        private readonly int _seed;
        
        public MountainsCommand(WorldCreator worldCreator, MountainBrush brush, List<Vector3Int> selectedCells, Grid grid, int seed) : base(worldCreator, brush, selectedCells, grid)
        {
            _seed = seed;
        }

        public override void Execute()
        {
            _data = _brush.Paint(_selectedCells, _grid, _seed);
        }
    }
}