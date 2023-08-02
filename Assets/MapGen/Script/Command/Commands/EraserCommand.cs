﻿using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.Eraser;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class EraserCommand : ICommand
    {
        private readonly WorldCreator _worldCreator;
        private readonly EraserBrush _brush;
        private readonly List<Vector3Int> _selectedCells;
        private readonly Grid _grid;
        private List<SpawnData> _cachedPlacables;

        public EraserCommand(WorldCreator worldCreator, EraserBrush brush, List<Vector3Int> selectedCells, Grid grid)
        {
            _worldCreator = worldCreator;
            _brush = brush;
            _selectedCells = selectedCells;
            _grid = grid;
        }
        
        public void Execute()
        {
            _cachedPlacables = _brush.GetSpawnData(_selectedCells, _grid);
            _brush.Erase(_selectedCells, _grid);
        }

        public void Undo()
        {
            foreach (var cachedPlacable in _cachedPlacables)
            {
                _worldCreator.SpawnObject(cachedPlacable);
            }
        }
    }
}