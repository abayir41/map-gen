using System.Collections.Generic;
using MapGen.Command;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class MultipleCellEditCommand : ICommand
    {
        private readonly WorldCreator _worldCreator;
        private readonly MultipleCellEditableBrush _groundBrush;
        private readonly List<Vector3Int> _selectedCells;
        private readonly Grid _grid;
        private List<SpawnData> _data;


        public MultipleCellEditCommand(WorldCreator worldCreator, MultipleCellEditableBrush groundBrush, List<Vector3Int> selectedCells, Grid grid)
        {
            _worldCreator = worldCreator;
            _groundBrush = groundBrush;
            _selectedCells = selectedCells;
            _grid = grid;
        }
        
        public void Execute()
        {
            _data = _groundBrush.Paint(_selectedCells, _grid);
        }

        public void Undo()
        {
            foreach (var spawnData in _data)
            {
                _worldCreator.DestroyByCellPoint(spawnData.SpawnPos);
            }
            
            _worldCreator.Grid.RegenerateShouldPlaceOnGrounds();
        }
    }
}