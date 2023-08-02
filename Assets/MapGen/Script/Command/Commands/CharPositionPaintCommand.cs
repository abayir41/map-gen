using MapGen.Command;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class CharPositionPaintCommand : ICommand
    {
        private CharPositionBrush _brush;
        private readonly Grid _grid;
        private Vector3Int _cachedPos;
        private Vector3Int _newPos;

        public CharPositionPaintCommand(CharPositionBrush brush, Grid grid, Vector3Int oldPos, Vector3Int newPos)
        {
            _brush = brush;
            _grid = grid;
            _cachedPos = oldPos;
            _newPos = newPos;
        }
        
        public void Execute()
        {
            _brush.Paint(_newPos, _grid);
        }

        public void Undo()
        {
            _brush.Paint(_cachedPos, _grid);
        }
    }
}