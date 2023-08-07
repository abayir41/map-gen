using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map.Brushes.Labyrinth
{
    public class LabyrinthBrushMazeCellHelper
    {
        public Dictionary<MazeCubicPositions, HashSet<Vector3Int>> PosVisualsDict { get; }
        private Vector2Int _startPoint;

        public LabyrinthBrushMazeCellHelper(Vector2Int startPoint, int wallThickness, int wayThickness, int wallHeight, int yStartLevel)
        {
            _startPoint = startPoint;
            PosVisualsDict = new Dictionary<MazeCubicPositions, HashSet<Vector3Int>>();
            var enums = Enum.GetValues(typeof(MazeCubicPositions));
            foreach (MazeCubicPositions position in enums)
            {
                PosVisualsDict.Add(position, new HashSet<Vector3Int>());
            }
            
            for (var x = 0; x < wallThickness + wayThickness + wallThickness; x++)
            {
                for (var z = 0; z < wallThickness + wayThickness + wallThickness; z++)
                {
                    for (var y = yStartLevel; y < wallHeight + yStartLevel; y++)
                    {
                        var position = FindMazeCubicPositionCorrespondingToPosition(new Vector2Int(x, z), wallThickness, wayThickness);
                        PosVisualsDict[position].Add(new Vector3Int(startPoint.x + x, y, startPoint.y + z));
                    }
                }
            }
        }

        private MazeCubicPositions FindMazeCubicPositionCorrespondingToPosition(Vector2Int pos, int wallThickness, int wayThickness)
        {
            if (pos.x >= 0 && pos.x < wallThickness)
            {
                if (pos.y >= 0 && pos.y < wallThickness)
                {
                    return MazeCubicPositions.BottomLeft;
                }

                if (pos.y >= wallThickness && pos.y < wallThickness + wayThickness)
                {
                    return MazeCubicPositions.Left;
                }

                if (pos.y >= wallThickness + wayThickness && pos.y < wallThickness + wayThickness + wallThickness)
                {
                    return MazeCubicPositions.TopLeft;
                }

                throw new Exception("Invalid Position");
            }
            
            if (pos.x >= wallThickness && pos.x < wallThickness + wayThickness)
            {
                if (pos.y >= 0 && pos.y < wallThickness)
                {
                    return MazeCubicPositions.Bottom;
                }

                if (pos.y >= wallThickness && pos.y < wallThickness + wayThickness)
                {
                    return MazeCubicPositions.Way;
                }

                if (pos.y >= wallThickness + wayThickness && pos.y < wallThickness + wayThickness + wallThickness)
                {
                    return MazeCubicPositions.Top;
                }
                
                throw new Exception("Invalid Position");
            }
            
            if (pos.x >= wallThickness + wayThickness && pos.x < wallThickness + wayThickness + wallThickness)
            {
                if (pos.y >= 0 && pos.y < wallThickness)
                {
                    return MazeCubicPositions.BottomRight;
                }

                if (pos.y >= wallThickness && pos.y < wallThickness + wayThickness)
                {
                    return MazeCubicPositions.Right;
                }

                if (pos.y >= wallThickness + wayThickness && pos.y < wallThickness + wayThickness + wallThickness)
                {
                    return MazeCubicPositions.TopRight;
                }
                
                throw new Exception("Invalid Position");
            }
            
            throw new Exception("Invalid Position");
        }
    }
}