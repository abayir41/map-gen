using MapGen.Placables;
using UnityEngine;

namespace MapGen.GridSystem
{
    public class GridElement
    {
        public Vector3Int Position { get; private set; }
        public int X => Position.x;
        public int Y => Position.y;
        public int Z => Position.z;

        public GridState GridState { get; private set; }
        
        public Placable PlacedItem { get; set; }

        public GridElement(int x, int y, int z)
        {
            Position = new Vector3Int(x, y, z);
            GridState = GridState.CanBeFilled;
        }

        public void MakeGridCanBeFilled()
        {
            GridState = GridState.CanBeFilled;
        }
        
        public void MakeGridCanBeFilledGround()
        {
            GridState = GridState.CanBeFilledGround;
        }

        public void FillGrid(Placable placable)
        {
            PlacedItem = placable;
            GridState = GridState.Filled;
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