using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Placables;
using Maze;
using Unity.VisualScripting;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Labyrinth
{
    [CreateAssetMenu(fileName = "Labyrinth Brush", menuName = "MapGen/Brushes/Labyrinth/Brush", order = 0)]
    public class LabyrinthBrush : Brush
    {
        [SerializeField] private LabyrinthBrushSettings _labyrinthBrushSettings;
        [SerializeField] private GroundBrush.GroundBrush _groundBrush;
        [SerializeField] private ObstacleSpawner.ObstaclesBrush _obstaclesBrush;
        
        private SelectedCellsHelper _selectedCellsHelper;
        private Grid _grid;

        public override string BrushName => "Labyrinth";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            var map = new RotationMap();
            _grid = grid;

            _groundBrush.Paint(selectedCells, grid);
            
            var labyrinthPieceSize = _labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness;
            var labyrinthPieceAmount = new Vector2Int((_selectedCellsHelper.XWidth - labyrinthPieceSize) / labyrinthPieceSize,
                (_selectedCellsHelper.ZWidth - labyrinthPieceSize) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, _labyrinthBrushSettings.RandomSettings.GetSeed());
            
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    var startPoint =
                        new Vector2Int(_selectedCellsHelper.MinX + x * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness),
                            _selectedCellsHelper.MinZ + z * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushHelper(startPoint, _labyrinthBrushSettings.WallThickness,
                        _labyrinthBrushSettings.WayThickness, _labyrinthBrushSettings.WallHeight, LabyrinthBrushSettings.LABYRINTH_START_Y_LEVEL);
                    
                    
                    var mazeCell = maze[x, z];

                    
                    for (var wayX = 0; wayX < _labyrinthBrushSettings.WayThickness; wayX++)
                    {
                        for (var wayY = 0; wayY < _labyrinthBrushSettings.WayThickness; wayY++)
                        {
                            if (!mazeCell.HasFlag(WallState.Left) && !mazeCell.HasFlag(WallState.Right))
                            {
                                map.SetCell(startPoint + Vector2Int.one * _labyrinthBrushSettings.WallThickness + new Vector2Int(wayX, wayY), new RotationMapCell(LabyrinthBrushSettings.HorizontalDegrees));
                            } 
                            else if (!mazeCell.HasFlag(WallState.Up) && !mazeCell.HasFlag(WallState.Down))
                            {
                                map.SetCell(startPoint + Vector2Int.one * _labyrinthBrushSettings.WallThickness + new Vector2Int(wayX, wayY), new RotationMapCell(LabyrinthBrushSettings.VerticalDegrees));
                            }
                        }
                    }
                    
                    
                    var cell = maze[x, z];

                    WallState? leftBottomWallState = null;
                    if (x > 0 && z > 0)
                    {
                        leftBottomWallState = maze[x - 1, z - 1];
                    }
                    WallState? bottomWallState = null;
                    if (z > 0)
                    {
                        bottomWallState = maze[x, z - 1];
                    }
                    WallState? leftWallState = null;
                    if (x > 0)
                    {
                        leftWallState = maze[x - 1, z];
                    }
                    

                    var up = cell.HasFlag(WallState.Up);
                    var left = cell.HasFlag(WallState.Left);
                    var right = cell.HasFlag(WallState.Right);
                    var bottom = cell.HasFlag(WallState.Down);
                    
                    if (up)
                    {
                        if (leftWallState == null || !leftWallState.HasFlag(WallState.Up))
                        {
                            OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft);
                        }
                        OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Top);
                        OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight);
                    }

                    if (left)
                    {
                        if (!up && (leftWallState == null || !leftWallState.HasFlag(WallState.Up)))
                        {
                            OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft);
                        }

                        if ((leftBottomWallState == null || !leftBottomWallState.HasFlag(WallState.Up)) &&
                            (bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) && 
                            (bottomWallState == null || !bottomWallState.HasFlag(WallState.Left)))
                        {
                            OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft);
                        }
                        
                        OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Left);
                    }

                    if (x == labyrinthPieceAmount.x - 1)
                    {
                        if (right)
                        {
                            if (!up)
                            {
                                OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight);
                            }
                            OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Right);

                            if ((bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) && (bottomWallState == null || !bottomWallState.HasFlag(WallState.Right)))
                            {
                                OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight);
                            }
                        }
                    }

                    if (z == 0)
                    {
                        if (bottom)
                        {
                            if (!right)
                            {
                                OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight);
                            }

                            if (!left && (leftWallState == null || !leftWallState.HasFlag(WallState.Down)))
                            {
                                OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft);
                            }
                            
                            OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Bottom);
                        }
                    }
                }
            }

            
            var oldMap = _obstaclesBrush.ObstacleBrushSettings.RotationMap;
            _obstaclesBrush.ObstacleBrushSettings.RotationMap = map;

            var yOffsetedCells = selectedCells.ConvertAll(pos => pos + Vector3Int.up * LabyrinthBrushSettings.LABYRINTH_START_Y_LEVEL);
            _obstaclesBrush.Paint(yOffsetedCells, grid);

            _obstaclesBrush.ObstacleBrushSettings.RotationMap = oldMap;
            
        }
        
        
        public void OpenWallIfNecessary(LabyrinthBrushHelper brushHelper, MazeCubicPositions position)
        {
            var positions = brushHelper.PosVisualsDict[position];
            
            foreach (var pos in positions)
            {
                if (_grid.IsCellExist(pos, out var cell) && cell.CellState != CellState.CanBeFilled) continue;

                WorldCreator.Instance.SpawnObject(pos, _labyrinthBrushSettings.MazeCubicGridPlacable, CellLayer.Ground, LabyrinthBrushSettings.WALL_ROTATION,position.ToString());
            }
        }
    }
}