using System.Collections.Generic;
using MapGen.GridSystem;
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

        
        public override string BrushName => "Labyrinth";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var map = new RotationMap();
            var selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            
            _groundBrush.Paint(selectedCells, grid);
            
            CreateLabyrinth(map, grid, selectedCellsHelper, out var probabilityMap);

            var obstaclesCells = selectedCells.ConvertAll(input =>
                input + Vector3Int.up * LabyrinthBrushSettings.OBSTACLES_START_Y_LEVEL);
            CreatObstacles(obstaclesCells, grid, map, probabilityMap);
        }

        private void CreateLabyrinth(RotationMap map, Grid grid, SelectedCellsHelper selectedCellsHelper, out float[,] obstacleProbabilityMap)
        {
            var labyrinthPieceSize = _labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness;
            var labyrinthPieceAmount = new Vector2Int((selectedCellsHelper.XWidth - labyrinthPieceSize) / labyrinthPieceSize,
                (selectedCellsHelper.ZWidth - labyrinthPieceSize) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, _labyrinthBrushSettings.RandomSettings.GetSeed());
            obstacleProbabilityMap = new float[selectedCellsHelper.XWidth, selectedCellsHelper.ZWidth];
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    var startWorldCellPoint =
                        new Vector2Int(selectedCellsHelper.MinX + x * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness),
                            selectedCellsHelper.MinZ + z * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushMazeCellHelper(startWorldCellPoint, _labyrinthBrushSettings.WallThickness,
                        _labyrinthBrushSettings.WayThickness, _labyrinthBrushSettings.WallHeight, LabyrinthBrushSettings.LABYRINTH_START_Y_LEVEL);
                    
                    var cell = maze[x, z];
                    
                    SetRotationMap(map, startWorldCellPoint, cell);

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
                        for (var way = 0; way < _labyrinthBrushSettings.WayThickness; way++)
                        {
                            var mazeCell = new Vector2Int(x , z) * labyrinthPieceSize 
                                              + Vector2Int.one * _labyrinthBrushSettings.WallThickness 
                                              + new Vector2Int(way,_labyrinthBrushSettings.WayThickness - 1);

                            obstacleProbabilityMap[mazeCell.x, mazeCell.y] =
                                _labyrinthBrushSettings.FirstEdgeObstacleProbability;
                        }
                    }
                    
                    if (left)
                    {
                        for (var way = 0; way < _labyrinthBrushSettings.WayThickness; way++)
                        {
                            var mazeCell = new Vector2Int(x , z) * labyrinthPieceSize 
                                           + Vector2Int.one * _labyrinthBrushSettings.WallThickness 
                                           + new Vector2Int(0,way);

                            obstacleProbabilityMap[mazeCell.x, mazeCell.y] =
                                _labyrinthBrushSettings.FirstEdgeObstacleProbability;
                        }
                    }

                    if (right)
                    {
                        for (var way = 0; way < _labyrinthBrushSettings.WayThickness; way++)
                        {
                            var mazeCell = new Vector2Int(x , z) * labyrinthPieceSize 
                                           + Vector2Int.one * _labyrinthBrushSettings.WallThickness 
                                           + new Vector2Int(_labyrinthBrushSettings.WayThickness - 1,way);

                            obstacleProbabilityMap[mazeCell.x, mazeCell.y] =
                                _labyrinthBrushSettings.FirstEdgeObstacleProbability;
                        }
                    }

                    if (bottom)
                    {
                        for (var way = 0; way < _labyrinthBrushSettings.WayThickness; way++)
                        {
                            var mazeCell = new Vector2Int(x , z) * labyrinthPieceSize 
                                           + Vector2Int.one * _labyrinthBrushSettings.WallThickness 
                                           + new Vector2Int(way,0);

                            obstacleProbabilityMap[mazeCell.x, mazeCell.y] =
                                _labyrinthBrushSettings.FirstEdgeObstacleProbability;
                        }
                    }
                    
                    
                    
                    
                    if (up)
                    {
                        if (leftWallState == null || !leftWallState.HasFlag(WallState.Up))
                        {
                            OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopLeft, grid);
                        }
                        OpenWall(labyrinthBrushHelper, MazeCubicPositions.Top, grid);
                        OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopRight, grid);
                    }

                    if (left)
                    {
                        if (!up && (leftWallState == null || !leftWallState.HasFlag(WallState.Up)))
                        {
                            OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopLeft, grid);
                        }

                        if ((leftBottomWallState == null || !leftBottomWallState.HasFlag(WallState.Up)) &&
                            (bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) && 
                            (bottomWallState == null || !bottomWallState.HasFlag(WallState.Left)))
                        {
                            OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, grid);
                        }
                        
                        OpenWall(labyrinthBrushHelper, MazeCubicPositions.Left, grid);
                    }

                    if (x == labyrinthPieceAmount.x - 1)
                    {
                        if (right)
                        {
                            if (!up)
                            {
                                OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopRight, grid);
                            }
                            OpenWall(labyrinthBrushHelper, MazeCubicPositions.Right, grid);

                            if ((bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) && (bottomWallState == null || !bottomWallState.HasFlag(WallState.Right)))
                            {
                                OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomRight, grid);
                            }
                        }
                    }

                    if (z == 0)
                    {
                        if (bottom)
                        {
                            if (!right)
                            {
                                OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomRight, grid);
                            }

                            if (!left && (leftWallState == null || !leftWallState.HasFlag(WallState.Down)))
                            {
                                OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, grid);
                            }
                            
                            OpenWall(labyrinthBrushHelper, MazeCubicPositions.Bottom, grid);
                        }
                    }
                }
            }
        }

        private void SetRotationMap(RotationMap map, Vector2Int startWorldCellPoint, WallState mazeCell)
        {
            for (var wayX = 0; wayX < _labyrinthBrushSettings.WayThickness; wayX++)
            {
                for (var wayY = 0; wayY < _labyrinthBrushSettings.WayThickness; wayY++)
                {
                    var gridCellPos = startWorldCellPoint + Vector2Int.one * _labyrinthBrushSettings.WallThickness + new Vector2Int(wayX, wayY);

                    if (!mazeCell.HasFlag(WallState.Left) && !mazeCell.HasFlag(WallState.Right))
                    {
                        var rotCell = new RotationMapCell(LabyrinthBrushSettings.HorizontalDegrees);
                        map.SetCell(gridCellPos, rotCell);
                    } 
                    else if (!mazeCell.HasFlag(WallState.Up) && !mazeCell.HasFlag(WallState.Down))
                    {
                        var rotCell = new RotationMapCell(LabyrinthBrushSettings.VerticalDegrees);
                        map.SetCell(gridCellPos, rotCell);                            
                    }
                }
            }
        }

        private void CreatObstacles(List<Vector3Int> selectedCells, Grid grid, RotationMap map, float[,] obstacleProbabilityMap)
        {
            var oldBoolSetting = _obstaclesBrush.ObstacleBrushSettings.UseNoiseMap;
            _obstaclesBrush.ObstacleBrushSettings.UseNoiseMap = false;
            _obstaclesBrush.ObstacleBrushSettings.ObjectPlacementProbability = obstacleProbabilityMap;
            
            var oldMap = _obstaclesBrush.ObstacleBrushSettings.RotationMap;
            _obstaclesBrush.ObstacleBrushSettings.RotationMap = map;

            _obstaclesBrush.Paint(selectedCells, grid);

            _obstaclesBrush.ObstacleBrushSettings.UseNoiseMap = oldBoolSetting;
            _obstaclesBrush.ObstacleBrushSettings.RotationMap = oldMap;
        }

        private void OpenWall(LabyrinthBrushMazeCellHelper brushMazeCellHelper, MazeCubicPositions position, Grid grid)
        {
            var positions = brushMazeCellHelper.PosVisualsDict[position];
            
            foreach (var pos in positions)
            {
                if (grid.IsCellExist(pos, out var cell) && cell.CellState != CellState.CanBeFilled) continue;

                WorldCreator.Instance.SpawnObject(pos, _labyrinthBrushSettings.MazeCubicGridPlacable, CellLayer.Ground, LabyrinthBrushSettings.WALL_ROTATION,position.ToString());
            }
        }
    }
}