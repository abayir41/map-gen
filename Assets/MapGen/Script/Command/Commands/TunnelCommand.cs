using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.TunnelBrush;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class TunnelCommand : ICommand
    {
        private readonly WorldCreator _worldCreator;
        private readonly TunnelBrush.TunnelBrush _brush;
        private readonly List<Vector3Int> _selectedCells;
        private readonly Grid _grid;
        private TunnelPaintData _cachedPlacables;

        public TunnelCommand(WorldCreator worldCreator, TunnelBrush.TunnelBrush brush, List<Vector3Int> selectedCells, Grid grid)
        {
            _worldCreator = worldCreator;
            _brush = brush;
            _selectedCells = selectedCells;
            _grid = grid;
        }
        
        public void Execute()
        {
            _cachedPlacables = _brush.Paint(_selectedCells, _grid);
            //_worldCreator.Grid.RegenerateShouldPlaceOnGrounds();
        }

        public void Undo()
        {
            foreach (var cachedPlacablesNewSpawnedObject in _cachedPlacables.NewSpawnedObjects)
            {
                _worldCreator.DestroyByData(cachedPlacablesNewSpawnedObject);
            }
            
            foreach (var cachedPlacable in _cachedPlacables.DestroyedObjects)
            {
                _worldCreator.SpawnObject(cachedPlacable);
            }
            
            _worldCreator.Grid.RegenerateShouldPlaceOnGrounds();
        }
    }
}