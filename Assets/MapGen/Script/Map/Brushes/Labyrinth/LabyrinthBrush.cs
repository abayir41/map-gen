using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.Random;
using Maze;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Labyrinth
{
    [CreateAssetMenu(fileName = "Labyrinth Brush", menuName = "MapGen/Brushes/Labyrinth/Brush", order = 0)]
    public class LabyrinthBrush : MultipleCellEditableBrush
    {
        [Header("Labyrinth Settings")]
        [SerializeField] private RandomSettings _randomSettings;
        [SerializeField] private int _wallThickness;
        [SerializeField] private int _wayThickness;
        [SerializeField] private int _wallHeight;
        [SerializeField] private Placable _mazeCubicGridPlacable;
        [SerializeField] [Range(0, 1)] private float firstEdgeObstacleProbability;
        [SerializeField] [Range(0, 1)] private float secondEdgeObstacleProbability;

        public const int WALL_ROTATION = 0;
        public const int LABYRINTH_START_Y_LEVEL = 1;
        public const int OBSTACLES_START_Y_LEVEL = 1;

        public static int[] HorizontalDegrees { get; } = { 0, 180 };
        public static int[] VerticalDegrees { get; } = { 90, 270 };
        [SerializeField] private GroundBrush.GroundBrush _groundBrush;
        [SerializeField] private ObstacleSpawner.ObstaclesBrush _obstaclesBrush;

        
        public Placable MazeCubicGridPlacable => _mazeCubicGridPlacable;
        public RandomSettings RandomSettings => _randomSettings;
        public int WallThickness => _wallThickness;
        public int WayThickness => _wayThickness;
        public int WallHeight => _wallHeight;
        public float FirstEdgeObstacleProbability => firstEdgeObstacleProbability;
        public float SecondEdgeObstacleProbability => secondEdgeObstacleProbability;
        
        public override string BrushName => "Labyrinth";

        public override void Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            var map = new RotationMap();
            var selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            
            _groundBrush.Paint(selectedCells, grid);
            
            CreateLabyrinth(map, grid, selectedCellsHelper, out var probabilityMap);

            var obstaclesCells = selectedCells.ConvertAll(input =>
                input + Vector3Int.up * OBSTACLES_START_Y_LEVEL);
            CreatObstacles(obstaclesCells, grid, map, probabilityMap);
        }

        private void CreateLabyrinth(RotationMap map, Grid grid, SelectedCellsHelper selectedCellsHelper, out float[,] obstacleProbabilityMap)
        {
            var labyrinthPieceSize = WallThickness + WayThickness;
            var labyrinthPieceAmount = new Vector2Int((selectedCellsHelper.XWidth - labyrinthPieceSize) / labyrinthPieceSize,
                (selectedCellsHelper.ZWidth - labyrinthPieceSize) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, RandomSettings.GetSeed());
            obstacleProbabilityMap = new float[selectedCellsHelper.XWidth, selectedCellsHelper.ZWidth];
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    var startWorldCellPoint =
                        new Vector2Int(selectedCellsHelper.MinX + x * (WallThickness + WayThickness),
                            selectedCellsHelper.MinZ + z * (WallThickness + WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushMazeCellHelper(startWorldCellPoint, WallThickness,
                        WayThickness, WallHeight, LABYRINTH_START_Y_LEVEL);
                    
                    var cell = maze[x, z];
                    
                    SetRotationMap(map, startWorldCellPoint, cell);

                    
                    var up = cell.HasFlag(WallState.Up);
                    var left = cell.HasFlag(WallState.Left);
                    var right = cell.HasFlag(WallState.Right);
                    var bottom = cell.HasFlag(WallState.Down);

                    SetObstacleProbabilityMap(obstacleProbabilityMap, up, bottom, right, left, x, z, labyrinthPieceSize);
                    LabyrinthProcess(grid, x, z, maze, up, bottom, left, right, labyrinthBrushHelper, labyrinthPieceAmount);
                }
            }
        }

        private void LabyrinthProcess(Grid grid, int x, int z, WallState[,] maze, bool up, bool bottom, bool left, bool right,
            LabyrinthBrushMazeCellHelper labyrinthBrushHelper, Vector2Int labyrinthPieceAmount)
        {
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

                    if ((bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) &&
                        (bottomWallState == null || !bottomWallState.HasFlag(WallState.Right)))
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

        private void SetObstacleProbabilityMap(float[,] obstacleProbabilityMap, bool up, bool bottom, bool right, bool left,
            int x, int z, int labyrinthPieceSize)
        {
            
            if (up && bottom)
            {
                if (UnityEngine.Random.value < 0.5)
                {
                    up = false;
                }
                else
                {
                    bottom = false;
                }
            }

            if (right && left)
            {
                if (UnityEngine.Random.value < 0.5)
                {
                    right = false;
                }
                else
                {
                    left = false;
                }
            }

            if (up)
            {
                for (var way = 0; way < WayThickness; way++)
                {
                    var mazeCell = new Vector2Int(x, z) * labyrinthPieceSize
                                   + Vector2Int.one * WallThickness
                                   + new Vector2Int(way, WayThickness - 1);


                    if (UnityEngine.Random.value < FirstEdgeObstacleProbability)
                        obstacleProbabilityMap[mazeCell.x, mazeCell.y] = 1;
                }
            }

            if (left)
            {
                for (var way = 0; way < WayThickness; way++)
                {
                    var mazeCell = new Vector2Int(x, z) * labyrinthPieceSize
                                   + Vector2Int.one * WallThickness
                                   + new Vector2Int(0, way);

                    if (UnityEngine.Random.value < FirstEdgeObstacleProbability)
                        obstacleProbabilityMap[mazeCell.x, mazeCell.y] = 1;
                }
            }

            if (right)
            {
                for (var way = 0; way < WayThickness; way++)
                {
                    var mazeCell = new Vector2Int(x, z) * labyrinthPieceSize
                                   + Vector2Int.one * WallThickness
                                   + new Vector2Int(WayThickness - 1, way);

                    if (UnityEngine.Random.value < FirstEdgeObstacleProbability)
                        obstacleProbabilityMap[mazeCell.x, mazeCell.y] = 1;
                }
            }

            if (bottom)
            {
                for (var way = 0; way < WayThickness; way++)
                {
                    var mazeCell = new Vector2Int(x, z) * labyrinthPieceSize
                                   + Vector2Int.one * WallThickness
                                   + new Vector2Int(way, 0);

                    if (UnityEngine.Random.value < FirstEdgeObstacleProbability)
                        obstacleProbabilityMap[mazeCell.x, mazeCell.y] = 1;
                }
            }
        }

        private void SetRotationMap(RotationMap map, Vector2Int startWorldCellPoint, WallState mazeCell)
        {
            for (var wayX = 0; wayX < WayThickness; wayX++)
            {
                for (var wayY = 0; wayY < WayThickness; wayY++)
                {
                    var gridCellPos = startWorldCellPoint + Vector2Int.one * WallThickness + new Vector2Int(wayX, wayY);

                    if (!mazeCell.HasFlag(WallState.Left) && !mazeCell.HasFlag(WallState.Right))
                    {
                        var rotCell = new RotationMapCell(HorizontalDegrees);
                        map.SetCell(gridCellPos, rotCell);
                    } 
                    else if (!mazeCell.HasFlag(WallState.Up) && !mazeCell.HasFlag(WallState.Down))
                    {
                        var rotCell = new RotationMapCell(VerticalDegrees);
                        map.SetCell(gridCellPos, rotCell);                            
                    }
                }
            }
        }

        private void CreatObstacles(List<Vector3Int> selectedCells, Grid grid, RotationMap map, float[,] obstacleProbabilityMap)
        {
            var oldBoolSetting = _obstaclesBrush.UseNoiseMap;
            _obstaclesBrush.UseNoiseMap = false;
            _obstaclesBrush.ObjectPlacementProbability = obstacleProbabilityMap;

            var oldMap = _obstaclesBrush.RotationMap;
            _obstaclesBrush.RotationMap = map;

            _obstaclesBrush.Paint(selectedCells, grid);

            _obstaclesBrush.UseNoiseMap = oldBoolSetting;
            _obstaclesBrush.RotationMap = oldMap;
        }

        private void OpenWall(LabyrinthBrushMazeCellHelper brushMazeCellHelper, MazeCubicPositions position, Grid grid)
        {
            var positions = brushMazeCellHelper.PosVisualsDict[position];
            
            foreach (var pos in positions)
            {
                if (grid.IsCellExist(pos, out var cell) && cell.CellState != CellState.CanBeFilled) continue;

                WorldCreator.Instance.SpawnObject(pos,MazeCubicGridPlacable, CellLayer.Ground, WALL_ROTATION,position.ToString());
            }
        }
    }
}