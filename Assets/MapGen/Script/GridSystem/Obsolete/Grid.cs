using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridCell = MapGen.GridSystem.Obsolete.GridCell;

namespace MapGen.GridSystem.Obsolete
{
    public class Grid
    {
        public GridCell[,,] Cells { get; }

        public int MaxX => Cells.GetLength(0);
        public int MaxY => Cells.GetLength(1);
        public int MaxZ => Cells.GetLength(2);
        
        public Grid(Vector3Int gridSize)
        {
            Cells = new GridCell[gridSize.x, gridSize.y, gridSize.z];

            for (int x = 0; x < MaxX; x++)
            {
                for (int y = 0; y < MaxY; y++)
                {
                    for (int z = 0; z < MaxZ; z++)
                    {
                        Cells[x, y, z] = new GridCell(x, y, z);
                    }
                }
            }
        }

        public GridCell GetCell(Vector3Int pos)
        {
            return Cells[pos.x, pos.y, pos.z];
        }

        public Vector3 GetWorldPosition(GridCell gridCell)
        {
            return gridCell.GlobalPosition;
        }

        public GridCell GetSameYAxisCell(GridCell cell, int yAxis)
        {
            return Cells[cell.GlobalPosition.x, yAxis, cell.GlobalPosition.z];
        }

        public List<GridCell> GetYAxis(GridCell cell)
        {
            return Enumerable.Range(0, Cells.GetLength(1))
                .Select(y => Cells[cell.GlobalPosition.x, y, cell.GlobalPosition.z])
                .ToList();
        }

        public List<GridCell> GetYAxisOfGrid(int y)
        {
            var result = new List<GridCell>();

            for (int x = 0; x < MaxX; x++)
            {
                for (int z = 0; z < MaxZ; z++)
                {
                    result.Add(Cells[x,y,z]);
                }
            }

            return result;
        }

        public bool OutSideOfGrid(Vector3Int pos)
        {
            return pos.x >= MaxX || pos.x < 0 || 
                   pos.y >= MaxY || pos.y < 0 ||
                   pos.z >= MaxZ || pos.z < 0;
        }
        
        public static Vector3Int RotateObstacleVector(float angle, Vector3Int vector3Int)
        {
            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var result = rotation * vector3Int;
            var resultAsVector3Int = new Vector3Int(Mathf.RoundToInt(result.x), Mathf.RoundToInt(result.y),
                Mathf.RoundToInt(result.z));

            return resultAsVector3Int;
        }

        public bool GetWithNeighbor(GridCell cell, Vector2Int xNeighbors, Vector2Int yNeighbors,
            Vector2Int zNeighbors, out List<GridCell> cells)
        {
            
            
            var minX = cell.GlobalPosition.x + xNeighbors.x;
            var maxX = cell.GlobalPosition.x + xNeighbors.y;
            
            var minY = cell.GlobalPosition.y + yNeighbors.x;
            var maxY = cell.GlobalPosition.y + yNeighbors.y;
            
            var minZ = cell.GlobalPosition.z + zNeighbors.x;
            var maxZ = cell.GlobalPosition.z + zNeighbors.y;

            var result = new List<GridCell>();
            for (int x = minX; x < maxX + 1; x++)
            {
                for (int y = minY; y < maxY + 1; y++)
                {
                    for (int z = minZ; z < maxZ + 1; z++)
                    {
                        var pos = new Vector3Int(x, y, z);
                        if (OutSideOfGrid(pos))
                        {
                            cells = null;
                            return false;
                        }
                        else
                        {
                            result.Add(GetCell(pos));
                        }
                    }
                }
            }
            
           
            
            cells = result;
            return true;
        }
    }
}