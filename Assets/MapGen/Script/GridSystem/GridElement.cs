using MapGen.Placables;
using UnityEngine;

namespace MapGen.GridSystem
{
    public class GridElement
    {
        public Vector3Int Position { get; }
        public GridState GridState { get; private set; }
        public GridLayer GridLayer { get; private set; }
        public Placable PlacedItem { get; private set; }

        public GridElement(int x, int y, int z)
        {
            Position = new Vector3Int(x, y, z);
            GridState = GridState.CanBeFilled;
            GridLayer = GridLayer.Empty;
        }

        public void FreeTheGrid()
        {
            GridState = GridState.CanBeFilled;
        }
        
        public void MakeGridCanBeFilledGround()
        {
            GridState = GridState.CanBeFilled;
            GridLayer = GridLayer.CanPlacableGround;
        }

        public void MakeGridEmpty()
        {
            GridLayer = GridLayer.Empty;
        }

        public void FillGrid(Placable placable, GridLayer gridLayer)
        {
            PlacedItem = placable;
            GridState = GridState.Filled;
            GridLayer = gridLayer;
        }

        public void LockGrid()
        {
            GridState = GridState.Locked;
        }

        public Vector3 GetWorldPosition()
        {
            return Position;
        }
    }
}