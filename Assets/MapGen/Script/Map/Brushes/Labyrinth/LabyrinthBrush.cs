using System.Collections.Generic;
using MapGen.Command;
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
    public class LabyrinthBrush : MultipleCellEditableBrush, IRandomBrush
    {
        [Header("Labyrinth Settings")]
        [SerializeField] private RandomSettings _randomSettings;
        [SerializeField] private int _wallThickness;
        [SerializeField] private int _wayThickness;
        [SerializeField] private int _wallHeight;
        [SerializeField] private Placable _mazeCubicGridPlacable;
        [SerializeField] private bool _openExits;
        [SerializeField] [Range(0, 1)] private float obstacleProbability;

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
        public float ObstacleProbability => obstacleProbability;
        
        public override string BrushName => "Labyrinth";
        protected override int HitBrushHeight => 1;

        public override ICommand GetPaintCommand(List<Vector3Int> selectedCells, Grid grid)
        {
            return new MultipleCellEditCommand(WorldCreator.Instance, this, selectedCells, grid);
        }

        public override List<SpawnData> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            UnityEngine.Random.InitState(RandomSettings.GetSeed());
            var map = new RotationMap();
            var selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            var result = new List<SpawnData>();
            
            var grounds = _groundBrush.Paint(selectedCells, grid);
            
            var labyrinth = CreateLabyrinth(map, grid, selectedCellsHelper, out var probabilityMap);

            var obstaclesCells = selectedCells.ConvertAll(input =>
                input + Vector3Int.up * OBSTACLES_START_Y_LEVEL);
            var obstacles = CreatObstacles(obstaclesCells, grid, map, probabilityMap);

            
            result.AddRange(grounds);
            result.AddRange(labyrinth);
            result.AddRange(obstacles);
            return result;
        }

        private List<SpawnData> CreateLabyrinth(RotationMap map, Grid grid, SelectedCellsHelper selectedCellsHelper, out float[,] obstacleProbabilityMap)
        {
            var result = new List<SpawnData>();
            var labyrinthPieceSize = WallThickness + WayThickness;
            var labyrinthPieceAmount = new Vector2Int((selectedCellsHelper.XWidth - WallThickness) / labyrinthPieceSize,
                (selectedCellsHelper.ZWidth - WallThickness) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, RandomSettings.GetSeed());

            if (_openExits)
            {
                OpenExits(maze);
            }
            obstacleProbabilityMap = new float[selectedCellsHelper.XWidth, selectedCellsHelper.ZWidth];
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    var startWorldCellPoint =
                        new Vector2Int(selectedCellsHelper.MinX + x * (WallThickness + WayThickness),
                            selectedCellsHelper.MinZ + z * (WallThickness + WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushMazeCellHelper(startWorldCellPoint, WallThickness,
                        WayThickness, WallHeight, LABYRINTH_START_Y_LEVEL + selectedCellsHelper.MinY);
                    
                    var cell = maze[x, z];
                    
                    SetRotationMap(map, startWorldCellPoint, cell);

                    
                    var up = cell.HasFlag(WallState.Up);
                    var left = cell.HasFlag(WallState.Left);
                    var right = cell.HasFlag(WallState.Right);
                    var bottom = cell.HasFlag(WallState.Down);

                    SetObstacleProbabilityMap(obstacleProbabilityMap, up, bottom, right, left, x, z, labyrinthPieceSize);
                    var labyrinth = LabyrinthProcess(grid, x, z, maze, up, bottom, left, right, labyrinthBrushHelper, labyrinthPieceAmount);
                    result.AddRange(labyrinth);
                }
            }

            return result;
        }

        private void OpenExits(WallState[,] maze)
        {
            maze[0, UnityEngine.Random.Range(0, maze.GetLength(1))] &= ~WallState.Left; // Remove left wall of leftmost cell
            maze[maze.GetLength(0)-1, UnityEngine.Random.Range(0, maze.GetLength(1))] &= ~WallState.Right; // Remove right wall of rightmost cell
        }

        private List<SpawnData> LabyrinthProcess(Grid grid, int x, int z, WallState[,] maze, bool up, bool bottom, bool left, bool right,
            LabyrinthBrushMazeCellHelper labyrinthBrushHelper, Vector2Int labyrinthPieceAmount)
        {
            var result = new List<SpawnData>();

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
                    var topLeft = OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopLeft, grid);
                    result.AddRange(topLeft);
                }

                var top = OpenWall(labyrinthBrushHelper, MazeCubicPositions.Top, grid);
                var topRight = OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopRight, grid);
                
                result.AddRange(top);
                result.AddRange(topRight);
            }

            if (left)
            {
                if (!up && (leftWallState == null || !leftWallState.HasFlag(WallState.Up)))
                {
                    var topLeft = OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopLeft, grid);
                    result.AddRange(topLeft);
                }

                if ((leftBottomWallState == null || !leftBottomWallState.HasFlag(WallState.Up)) &&
                    (bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) &&
                    (bottomWallState == null || !bottomWallState.HasFlag(WallState.Left)))
                {
                    var bottomLeft = OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, grid);
                    result.AddRange(bottomLeft);
                }

                var leftData = OpenWall(labyrinthBrushHelper, MazeCubicPositions.Left, grid);
                result.AddRange(leftData);
            }

            if (x == labyrinthPieceAmount.x - 1)
            {
                if (right)
                {
                    if (!up)
                    {
                        var topRight = OpenWall(labyrinthBrushHelper, MazeCubicPositions.TopRight, grid);
                        result.AddRange(topRight);
                    }

                    var rightData = OpenWall(labyrinthBrushHelper, MazeCubicPositions.Right, grid);
                    result.AddRange(rightData);
                    
                    if ((bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) &&
                        (bottomWallState == null || !bottomWallState.HasFlag(WallState.Right)))
                    {
                        var bottomRight = OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomRight, grid);
                        result.AddRange(bottomRight);
                    }
                }
            }

            if (z == 0)
            {
                if (bottom)
                {
                    if (!right)
                    {
                        var bottomRight = OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomRight, grid);
                        result.AddRange(bottomRight);
                    }

                    if (!left && (leftWallState == null || !leftWallState.HasFlag(WallState.Down)))
                    {
                        var bottomLeft = OpenWall(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, grid);
                        result.AddRange(bottomLeft);
                    }

                    var bottomData = OpenWall(labyrinthBrushHelper, MazeCubicPositions.Bottom, grid);
                    result.AddRange(bottomData);
                }
            }

            return result;
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


                    if (UnityEngine.Random.value < ObstacleProbability)
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

                    if (UnityEngine.Random.value < ObstacleProbability)
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

                    if (UnityEngine.Random.value < ObstacleProbability)
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

                    if (UnityEngine.Random.value < ObstacleProbability)
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

        private List<SpawnData> CreatObstacles(List<Vector3Int> selectedCells, Grid grid, RotationMap map, float[,] obstacleProbabilityMap)
        {
            var oldBoolSetting = _obstaclesBrush.UseNoiseMap;
            _obstaclesBrush.UseNoiseMap = false;
            _obstaclesBrush.ObjectPlacementProbability = obstacleProbabilityMap;

            var oldMap = _obstaclesBrush.RotationMap;
            _obstaclesBrush.RotationMap = map;

            var data = _obstaclesBrush.Paint(selectedCells, grid);

            _obstaclesBrush.UseNoiseMap = oldBoolSetting;
            _obstaclesBrush.RotationMap = oldMap;

            return data;
        }

        private List<SpawnData> OpenWall(LabyrinthBrushMazeCellHelper brushMazeCellHelper, MazeCubicPositions position, Grid grid)
        {
            var result = new List<SpawnData>();
            var positions = brushMazeCellHelper.PosVisualsDict[position];
            
            foreach (var pos in positions)
            {
                if (grid.IsCellExist(pos, out var cell) && cell.CellState != CellState.CanBeFilled) continue;

                var data = new SpawnData(pos, MazeCubicGridPlacable, WALL_ROTATION, CellLayer.Ground, pos.ToString());
                WorldCreator.Instance.SpawnObject(data);
                result.Add(data);
            }

            return result;
        }
    }
}