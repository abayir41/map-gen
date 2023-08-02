using System.Collections.Generic;

namespace MapGen.GridSystem
{
    public class PlacableGrids
    {
        public List<GridCell> PhysicalCells { get; }
        public List<GridCell> NewGroundCells { get; }
        public List<GridCell> ShouldPlacedOnCells { get; }

        public PlacableGrids(List<GridCell> physicalCells, List<GridCell> newGroundCells, List<GridCell> shouldPlacedOnCells)
        {
            PhysicalCells = physicalCells;
            NewGroundCells = newGroundCells;
            ShouldPlacedOnCells = shouldPlacedOnCells;
        }
    }
}