using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.Labyrinth;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class LabyrinthCommand : ICommand
    {
        private readonly LabyrinthBrush _brush;
        private readonly ICommand _groundCommand;
        private readonly WorldCreator _worldCreator;
        private readonly List<Vector3Int> _selectedCells;
        private readonly Grid _grid;
        private int _cachedSeed;
        private List<SpawnData> _data;

        public LabyrinthCommand(LabyrinthBrush brush, ICommand groundCommand, WorldCreator worldCreator, List<Vector3Int> selectedCells, Grid grid, int seed)
        {
            _brush = brush;
            _groundCommand = groundCommand;
            _worldCreator = worldCreator;
            _selectedCells = selectedCells;
            _grid = grid;
            _cachedSeed = seed;
        }
        
        public void Execute()
        {
            _groundCommand?.Execute();
            _data = _brush.Paint(_selectedCells, _grid, _cachedSeed);
        }

        public void Undo()
        {
            foreach (var spawnData in _data)
            {
                _worldCreator.DestroyByCellPoint(spawnData.SpawnPos);
            }
            
            _groundCommand?.Undo();
            
            _worldCreator.Grid.RegenerateShouldPlaceOnGrounds();
        }
    }
}