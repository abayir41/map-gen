using System.Collections.Generic;
using MapGen.Command;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public abstract class MultipleCellBrushCommandBase<TBrush> : ICommand
    {
        protected readonly WorldCreator _worldCreator;
        protected readonly TBrush _brush;
        protected readonly List<Vector3Int> _selectedCells;
        protected readonly Grid _grid;
        protected List<SpawnData> _data;


        public MultipleCellBrushCommandBase(WorldCreator worldCreator, TBrush brush, List<Vector3Int> selectedCells, Grid grid)
        {
            _worldCreator = worldCreator;
            _brush = brush;
            _selectedCells = selectedCells;
            _grid = grid;
        }

        public abstract void Execute();

        public virtual void Undo()
        {
            foreach (var spawnData in _data)
            {
                _worldCreator.DestroyByCellPoint(spawnData.SpawnPos);
            }
            
            _worldCreator.Grid.RegenerateShouldPlaceOnGrounds();
        }
    }
}