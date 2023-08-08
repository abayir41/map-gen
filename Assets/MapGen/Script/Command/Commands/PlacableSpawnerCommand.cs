using MapGen.Command;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class PlacableSpawnerCommand : ICommand
    {
        private readonly PlacableSpawner _placableSpawner;
        private readonly WorldCreator _creator;
        private readonly Vector3Int _startPoint;
        private readonly Grid _grid;
        private SpawnData? _data;

        public PlacableSpawnerCommand(PlacableSpawner placableSpawner, WorldCreator creator, Vector3Int startPoint, Grid grid)
        {
            _placableSpawner = placableSpawner;
            _creator = creator;
            _startPoint = startPoint;
            _grid = grid;
        }
        
        public void Execute()
        {
            _data = _placableSpawner.Paint(_startPoint, _grid);
        }

        public void Undo()
        {
            if(!_data.HasValue) return;
            
            _creator.DestroyByData(_data.Value);
        }
    }
}