﻿using System;
using System.Collections.Generic;

namespace Maze
{
    [Flags]
    public enum WallState
    {
        // 0000 -> NO WALLS
        // 1111 -> LEFT,RIGHT,UP,DOWN
        Left = 1, // 0001
        Right = 2, // 0010
        Up = 4, // 0100
        Down = 8, // 1000

        Visited = 128, // 1000 0000
    }

    public struct Position
    {
        public int X;
        public int Y;
    }

    public struct Neighbour
    {
        public Position Position;
        public WallState SharedWall;
    }

    public class MazeGenerator
    {
        private static WallState GetOppositeWall(WallState wall)
        {
            switch (wall)
            {
                case WallState.Right:
                    return WallState.Left;
                case WallState.Left:
                    return WallState.Right;
                case WallState.Up:
                    return WallState.Down;
                case WallState.Down:
                    return WallState.Up;
                default:
                    return WallState.Left;
            }
        }

        private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height, int seed)
        {
            // here we make changes
            var rng = new System.Random(seed);
            var positionStack = new Stack<Position>();
            var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };

            maze[position.X, position.Y] |= WallState.Visited; // 1000 1111
            positionStack.Push(position);

            while (positionStack.Count > 0)
            {
                var current = positionStack.Pop();
                var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

                if (neighbours.Count > 0)
                {
                    positionStack.Push(current);

                    var randIndex = rng.Next(0, neighbours.Count);
                    var randomNeighbour = neighbours[randIndex];

                    var nPosition = randomNeighbour.Position;
                    maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                    maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                    maze[nPosition.X, nPosition.Y] |= WallState.Visited;

                    positionStack.Push(nPosition);
                }
            }

            return maze;
        }

        private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
        {
            var list = new List<Neighbour>();

            if (p.X > 0) // left
            {
                if (!maze[p.X - 1, p.Y].HasFlag(WallState.Visited))
                {
                    list.Add(new Neighbour
                    {
                        Position = new Position { X = p.X - 1, Y = p.Y }, SharedWall = WallState.Left
                    });
                }
            }

            if (p.Y > 0) // DOWN
            {
                if (!maze[p.X, p.Y - 1].HasFlag(WallState.Visited))
                {
                    list.Add(new Neighbour
                    {
                        Position = new Position { X = p.X, Y = p.Y - 1 }, SharedWall = WallState.Down
                    });
                }
            }

            if (p.Y < height - 1) // UP
            {
                if (!maze[p.X, p.Y + 1].HasFlag(WallState.Visited))
                {
                    list.Add(new Neighbour
                    {
                        Position = new Position { X = p.X, Y = p.Y + 1 }, SharedWall = WallState.Up
                    });
                }
            }

            if (p.X < width - 1) // RIGHT
            {
                if (!maze[p.X + 1, p.Y].HasFlag(WallState.Visited))
                {
                    list.Add(new Neighbour
                    {
                        Position = new Position { X = p.X + 1, Y = p.Y }, SharedWall = WallState.Right
                    });
                }
            }

            return list;
        }

        public static WallState[,] Generate(int width, int height, int seed)
        {
            WallState[,] maze = new WallState[width, height];
            WallState initial = WallState.Right | WallState.Left | WallState.Up | WallState.Down;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    maze[i, j] = initial; // 1111
                }
            }

            return ApplyRecursiveBacktracker(maze, width, height, seed);
        }
    }
}