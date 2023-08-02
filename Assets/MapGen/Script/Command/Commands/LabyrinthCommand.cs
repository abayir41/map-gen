﻿using System.Collections.Generic;
using MapGen.Command;
using MapGen.Map.Brushes.Labyrinth;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class LabyrinthCommand : ICommand
    {
        private readonly LabyrinthBrush _obstaclesBrush;
        private readonly List<Vector3Int> _selectedCells;
        private readonly Grid _grid;
        private readonly WorldCreator _creator;
        private List<SpawnData> _spawnData;

        public LabyrinthCommand(LabyrinthBrush obstaclesBrush, List<Vector3Int> selectedCells, Grid grid, WorldCreator creator)
        {
            _obstaclesBrush = obstaclesBrush;
            _selectedCells = selectedCells;
            _grid = grid;
            _creator = creator;
        }
        
        public void Execute()
        {
            _spawnData = _obstaclesBrush.Paint(_selectedCells, _grid);
        }

        public void Undo()
        {
            foreach (var spawnData in _spawnData)
            {
                _creator.DestroyByCellPoint(spawnData.SpawnPos);
            }
        }
    }
}