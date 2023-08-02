using MapGen.Map;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.GridSystem
{
    public class GridCell
    {
        public Vector3Int CellPosition { get; }
        public Vector3 WorldPosition { get; }
        public CellState CellState { get; private set; }
        public CellLayer CellLayer { get; private set; }
        public Placable Item { get; private set; }
        
        public GridCell(Vector3Int cellPosition, Vector3 worldPosition)
        {
            CellPosition = cellPosition;
            WorldPosition = worldPosition;
        }
        
        public void FreeTheCell()
        {
            Item = null;
            CellState = CellState.CanBeFilled;
        }
        
        public void MakeCellCanBeFilledGround()
        {
            Item = null;
            CellState = CellState.CanBeFilled;
            CellLayer = CellLayer.CanPlacableGround;
        }

        public void MakeCellEmpty()
        {
            Item = null;
            CellLayer = CellLayer.Empty;
        }

        public void FillCell(Placable placable, CellLayer cellLayer)
        {
            Item = placable;
            CellState = CellState.Filled;
            CellLayer = cellLayer;
        }

        public void LockCell()
        {
            CellState = CellState.Locked;
        }
    }
}