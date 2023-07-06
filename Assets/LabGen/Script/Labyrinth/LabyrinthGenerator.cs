using MapGen.Random;
using MapGen.Utilities;
using Maze;
using UnityEngine;

namespace LabGen.Labyrinth
{
    public class LabyrinthGenerator : MonoBehaviour
    {
        [SerializeField] private LabyrinthSettings _labyrinthSettings;
        [SerializeField] private Transform _mazeGridParent;
        public LabyrinthSettings LabyrinthSettings => _labyrinthSettings;
        
        public void GenerateLabyrinth()
        {
            var maze = MazeGenerator.Generate(_labyrinthSettings.MapSize.x, _labyrinthSettings.MapSize.y, _labyrinthSettings.RandomSettings.GetSeed());
            
            for (int x = 0; x < _labyrinthSettings.MapSize.x; ++x)
            {
                for (int y = 0; y < _labyrinthSettings.MapSize.y; ++y)
                {
                    var cell = maze[x, y];
                    var floor = Instantiate(_labyrinthSettings.MazeGridPlacable, _mazeGridParent);
                    floor.transform.position = new Vector3(x, 0, y);

                    if (cell.HasFlag(WallState.Up))
                    {
                        floor.OpenTopWall();
                    }

                    if (cell.HasFlag(WallState.Left))
                    {
                        floor.OpenLeftWall();
                    }

                    if (x == _labyrinthSettings.MapSize.x - 1)
                    {
                        if (cell.HasFlag(WallState.Right))
                        {
                            floor.OpenRightWall();
                        }
                    }

                    if (y == 0)
                    {
                        if (cell.HasFlag(WallState.Down))
                        {
                            floor.OpenBottomWall();
                        }
                    }
                }

            }
        }
    }
}