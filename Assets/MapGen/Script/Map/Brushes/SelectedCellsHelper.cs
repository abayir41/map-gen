using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    public class SelectedCellsHelper
    {
        public Grid Grid { get; }
        public List<Vector3Int> Cells { get; }

        public int MaxX { get; }
        public int MaxY { get; }
        public int MaxZ { get; }

        public int MinX { get; }
        public int MinY { get; }
        public int MinZ { get; }
        
        public int XWidth { get; }
        public int ZWidth { get; }

        public SelectedCellsHelper(List<Vector3Int> cells, Grid grid)
        {
            Grid = grid;
            Cells = cells;
            
            MaxX = Cells.Max(cell => cell.x);
            MaxY = Cells.Max(cell => cell.y);
            MaxZ = Cells.Max(cell => cell.z);

            MinX = Cells.Min(cell => cell.x);
            MinY = Cells.Min(cell => cell.y);
            MinZ = Cells.Min(cell => cell.z);

            XWidth = MaxX - MinX;
            ZWidth = MaxZ - MinZ;
        }
        
        public List<Vector3Int> GetYAxisOfGrid(int y)
        {
            return Cells.Where(cell => cell.y == y).ToList();
        }
    }
}