using System;
using System.Collections.Generic;
using MapGen.Placables;

namespace MapGen.GridSystem
{
    public class PlacableGrids
    {
        public Dictionary<PlacableCellType, List<GridCell>> Cells { get; private set; } = new();

        public PlacableGrids()
        {
            foreach (PlacableCellType cellType in Enum.GetValues(typeof(PlacableCellType)))
            {
                Cells.Add(cellType, new List<GridCell>());
            }
        }
        
        public void Add(PlacableCellType cellType, GridCell cell)
        {
            Cells[cellType].Add(cell);
        }
    }
}