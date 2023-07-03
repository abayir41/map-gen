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
        public GridElement PlacedItemOwner { get; private set; }

        public GridElement(int x, int y, int z)
        {
            Position = new Vector3Int(x, y, z);
            GridState = GridState.Neutral;
        }

        public void MakeGridCanBeFilled()
        {
            GridState = GridState.CanBeFilled;
        }

        public void FillGrid()
        {
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