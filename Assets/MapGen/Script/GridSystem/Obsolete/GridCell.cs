using MapGen.Placables;
using UnityEngine;

namespace MapGen.GridSystem.Obsolete
{
    public class GridCell
    {
        public Vector3Int GlobalPosition { get; }
        public Vector3Int LocalPosition { get; }
        public CellState CellState { get; private set; }
        public CellLayer CellLayer { get; private set; }
        public Placable PlacedItem { get; private set; }

        public GridCell(int x, int y, int z)
        {
            GlobalPosition = new Vector3Int(x, y, z);
            CellState = CellState.CanBeFilled;
            CellLayer = CellLayer.Empty;
        }

        public void FreeTheCell()
        {
            CellState = CellState.CanBeFilled;
        }
        
        public void MakeCellCanBeFilledGround()
        {
            CellState = CellState.CanBeFilled;
            CellLayer = CellLayer.CanPlacableGround;
        }

        public void MakeCellEmpty()
        {
            CellLayer = CellLayer.Empty;
        }

        public void FillCell(Placable placable, CellLayer cellLayer)
        {
            PlacedItem = placable;
            CellState = CellState.Filled;
            CellLayer = cellLayer;
        }

        public void LockCell()
        {
            CellState = CellState.Locked;
        }

        public Vector3 GetWorldPosition()
        {
            return GlobalPosition;
        }
    }
}