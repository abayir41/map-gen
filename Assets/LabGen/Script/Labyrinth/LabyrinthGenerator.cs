using System;
using System.Collections;
using MapGen.Random;
using MapGen.Utilities;
using Maze;
using UnityEngine;
using Weaver;

namespace LabGen.Labyrinth
{
    public class LabyrinthGenerator : MonoBehaviour
    {
        [SerializeField] private LabyrinthSettings _labyrinthSettings;
        [SerializeField] private Transform _mazeGridParent;
        public LabyrinthSettings LabyrinthSettings => _labyrinthSettings;
        
        [MethodTimer]
        public void GenerateLabyrinth()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Method Should be called when game is playing, It can't be called from Editor");
                return;
            }

            var maze = MazeGenerator.Generate(_labyrinthSettings.MapSize.x, _labyrinthSettings.MapSize.y, _labyrinthSettings.RandomSettings.GetSeed());
            
            for (int x = 0; x < _labyrinthSettings.MapSize.x; x++)
            {
                for (int y = 0; y < _labyrinthSettings.MapSize.y; y++)
                {
                    var cell = maze[x, y];

                    var floor = Instantiate(_labyrinthSettings.MazeCubicGridPlacable, _mazeGridParent);
                    floor.gameObject.name = $"({x}, {y})";
                    floor.SetupMazePlacable(_labyrinthSettings.WallThickness, _labyrinthSettings.WayThickness, _labyrinthSettings.WallHeight);
                    var xWorldPos = x * (_labyrinthSettings.WallThickness + _labyrinthSettings.WayThickness);
                    var zWorldPos = y * (_labyrinthSettings.WallThickness + _labyrinthSettings.WayThickness);
                    floor.transform.position = new Vector3(xWorldPos , 0, zWorldPos);

                    var result = $"({x}, {y}): ";
                    foreach (WallState state in cell.GetFlags())
                    {
                        result += state + ", ";
                    }
                    
                    if (cell.HasFlag(WallState.Up))
                    {
                        var upCellPos = new Vector2Int(x, y + 1);
                        var leftCellPos = new Vector2Int(x - 1, y);

                        if (IsOutsideOfMap(upCellPos, maze) && IsOutsideOfMap(leftCellPos, maze))
                        {
                            floor.OpenTopLeftWall();
                        }
                        else if (!IsOutsideOfMap(upCellPos, maze) && !maze[upCellPos.x, upCellPos.y].HasFlag(WallState.Left))
                        {
                            floor.OpenTopLeftWall();
                        }
                        else if (!IsOutsideOfMap(leftCellPos, maze) && !maze[leftCellPos.x, leftCellPos.y].HasFlag(WallState.Up))
                        {
                            floor.OpenTopLeftWall();
                        }
                        
                        floor.OpenTopWall();
                        floor.OpenTopRightWall();
                    }

                    if (cell.HasFlag(WallState.Left))
                    {
                        var upCellPos = new Vector2Int(x, y - 1);
                        var leftCellPos = new Vector2Int(x - 1, y);
                        
                        if (IsOutsideOfMap(upCellPos, maze) && IsOutsideOfMap(leftCellPos, maze))
                        {
                            floor.OpenTopLeftWall();
                        }
                        else if (!IsOutsideOfMap(upCellPos, maze) && !maze[upCellPos.x, upCellPos.y].HasFlag(WallState.Left))
                        {
                            floor.OpenTopLeftWall();
                        }
                        else if (!IsOutsideOfMap(leftCellPos, maze) && !maze[leftCellPos.x, leftCellPos.y].HasFlag(WallState.Up))
                        {
                            floor.OpenTopLeftWall();
                        }
                        
                        floor.OpenLeftWall();
                        floor.OpenBottomLeftWall();
                    }

                    if (x == _labyrinthSettings.MapSize.x - 1)
                    {
                        if (cell.HasFlag(WallState.Right))
                        {
                            floor.OpenTopRightWall();
                            floor.OpenRightWall();
                            floor.OpenBottomRightWall();
                        }
                    }

                    if (y == 0)
                    {
                        if (cell.HasFlag(WallState.Down))
                        {
                            floor.OpenBottomLeftWall();
                            floor.OpenBottomWall();
                            floor.OpenBottomRightWall();
                        }
                    }
                }
            }
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