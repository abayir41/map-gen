using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using LabGen.Labyrinth;
using LabGen.Placables;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushHelper;
using MapGen.Placables;
using Maze;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    [CreateAssetMenu(fileName = "Labyrinth Brush", menuName = "MapGen/Brushes/Labyrinth/Brush", order = 0)]
    public class LabyrinthBrush : Brush
    {
        [SerializeField] private LabyrinthBrushSettings _labyrinthBrushSettings;

        public LabyrinthBrushSettings LabyrinthBrushSettings => _labyrinthBrushSettings;

        private SelectedCellsHelper _selectedCellsHelper;
        private Grid _grid;
        private List<Vector3Int> _groundCells;
        private List<Vector3Int> _selectedCells;
        private TimeSpan ASD;
        private TimeSpan BNN;
        private TimeSpan YUN;
        
        public override List<Placable> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            
            var result = new List<Placable>();
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            _groundCells = _selectedCellsHelper.GetYAxisOfGrid(LabyrinthBrushSettings.GROUND_Y_LEVEL);
            _grid = grid;
            _selectedCells = selectedCells;

            result.AddRange(CreateGround());
            
            var labyrinthPieceSize = _labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness;
            var labyrinthPieceAmount = new Vector2Int((_selectedCellsHelper.XWidth - labyrinthPieceSize) / labyrinthPieceSize,
                (_selectedCellsHelper.ZWidth - labyrinthPieceSize) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, _labyrinthBrushSettings.RandomSettings.GetSeed());

            var dict = new Dictionary<int, int>();
            var watch = new Stopwatch();
            var it = 0;
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    watch.Reset();
                    watch.Start();
                    
                    var startPoint =
                        new Vector2Int(_selectedCellsHelper.MinX + x * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness),
                            _selectedCellsHelper.MinZ + z * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushHelper(startPoint, _labyrinthBrushSettings.WallThickness,
                        _labyrinthBrushSettings.WayThickness, _labyrinthBrushSettings.WallHeight);
                    
                    
                    var cell = maze[x, z];
                    if (cell.HasFlag(WallState.Up))
                    {
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Top, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight, result));
                    }

                    if (cell.HasFlag(WallState.Left))
                    {
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Left, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, result));
                    }

                    if (x == labyrinthPieceAmount.x - 1)
                    {
                        if (cell.HasFlag(WallState.Right))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Right, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight, result));
                        }
                    }

                    if (z == 0)
                    {
                        if (cell.HasFlag(WallState.Down))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Bottom, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, result));
                        }
                    }
                    
                    it++;
                    watch.Stop();
                    dict.Add(it, watch.Elapsed.Milliseconds);
                }
            }
            
            
            Debug.Log($"1: {ASD.TotalMilliseconds}, 2: {BNN.TotalMilliseconds}, 3: {YUN.TotalMilliseconds}");
            
            return result;
        }
        
        private List<Placable> CreateGround()
        {
            var result = new List<Placable>();
            foreach (var selectedCell in _groundCells)
            {
                if (_grid.IsCellExist(selectedCell, out var cell))
                {
                    cell.MakeCellCanBeFilledGround();
                }
                var placable =  WorldCreator.Instance.SpawnObject(selectedCell, _labyrinthBrushSettings.Ground, CellLayer.Ground, LabyrinthBrushSettings.GROUND_ROTATION);
                result.Add(placable);
            }

            return result;
        }

        
        
        public List<Placable> OpenWallIfNecessary(LabyrinthBrushHelper brushHelper, MazeCubicPositions position, List<Placable> alreadyPlacedPlacables)
        {
            var positions = brushHelper.PosVisualsDict[position];
            var result = new List<Placable>();

            var watch = new Stopwatch();
            
            foreach (var pos in positions)
            {
                
                watch.Start();
                if (alreadyPlacedPlacables.Any(item => item.GridPos == pos))
                {
                    watch.Stop();
                    ASD = ASD.Add(watch.Elapsed);
                    watch.Reset();
                    continue;
                }
                else
                {
                    watch.Stop();
                    ASD = ASD.Add(watch.Elapsed);
                    watch.Reset();
                }

                watch.Start();
                if (_grid.IsCellExist(pos, out var cell))
                {
                    watch.Stop();
                    BNN = BNN.Add(watch.Elapsed);
                    watch.Reset();
                    cell.MakeCellCanBeFilledGround();
                }
                else
                {
                    watch.Stop();
                    BNN = BNN.Add(watch.Elapsed);
                    watch.Reset();
                }
                
                watch.Start();
                var placable = WorldCreator.Instance.SpawnObject(pos, _labyrinthBrushSettings.MazeCubicGridPlacable, CellLayer.Ground,
                    LabyrinthBrushSettings.GROUND_ROTATION,position.ToString());
                result.Add(placable);

                watch.Stop();
                YUN = YUN.Add(watch.Elapsed);
                watch.Reset();

            }
            
            return result;
        }

        private bool IsOutsideOfMap<T>(Vector2Int pos, T[,] wallStates)
        {
            return IsOutsideOfMap(pos.x, pos.y, wallStates);
        }
        
        private bool IsOutsideOfMap<T>(int x, int y, T[,] wallStates)
        {
            return x < 0 || x >= wallStates.GetLength(0) || y < 0 || y >= wallStates.GetLength(1);
        }
    }
}