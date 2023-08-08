using System.Collections.Generic;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class GroundBrushCommand : MultipleCellBrushCommandBase<GroundBrush.GroundBrush>
    {
        public GroundBrushCommand(WorldCreator worldCreator, GroundBrush.GroundBrush brush, List<Vector3Int> selectedCells, Grid grid) : base(worldCreator, brush, selectedCells, grid)
        {
        }

        public override void Execute()
        {
            _data = _brush.Paint(_selectedCells, _grid);
        }
    }
}