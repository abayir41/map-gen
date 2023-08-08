using MapGen.Command;
using UnityEngine;

namespace MapGen.Map.Brushes
{
    public class MapCommand : ICommand
    {
        private readonly ICommand _groundCommand;
        private readonly ICommand _mountainsCommand;
        private readonly ICommand _tunnels;
        private readonly ICommand _obstacles;

        public MapCommand(ICommand groundCommand, ICommand mountainsCommand, ICommand tunnels, ICommand obstacles)
        {
            _groundCommand = groundCommand;
            _mountainsCommand = mountainsCommand;
            _tunnels = tunnels;
            _obstacles = obstacles;
        }
        
        public void Execute()
        {
            _groundCommand?.Execute();
            _mountainsCommand?.Execute();
            _tunnels?.Execute();
            _obstacles?.Execute();
        }

        public void Undo()
        {
            _obstacles?.Undo();
            _tunnels?.Undo();
            _mountainsCommand?.Undo();
            _groundCommand?.Undo();
        }
    }
}