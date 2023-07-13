using System.Collections.Generic;

namespace MapGen.GridSystem
{
    public class PlacableGrids
    {
        public List<GridCell> PhysicalCells { get; }
        public List<GridCell> NewGroundCells { get; }
        
        public PlacableGrids(List<GridCell> physicalCells, List<GridCell> newGroundCells)
        {
            PhysicalCells = physicalCells;
            NewGroundCells = newGroundCells;
        }
    }
}