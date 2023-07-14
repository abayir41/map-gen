using System.Collections.Generic;
using MapGen.Map;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.GridSystem
{
    public class Grid
    {
        private Dictionary<Placable, PlacableGrids> ItemCellsDict { get; }

        private Dictionary<Vector3Int, GridCell> CachedCells { get; } = new();

        public Grid()
        {
            ItemCellsDict = new Dictionary<Placable, PlacableGrids>();
        }

        public GridCell GetCell(Vector3Int cellPosition)
        {
            return CachedCells[cellPosition];
        }

        public Vector3 CellPositionToRealWorld(Vector3Int cellPos)
        {
            return WorldSettings.GridCellRealWorldSize * cellPos;
        }

        public Vector3Int RealWorldToCellPosition(Vector3 realWorldPos)
        {
            var x = Mathf.FloorToInt(realWorldPos.x / WorldSettings.GridCellRealWorldSize.y);
            var y = Mathf.FloorToInt(realWorldPos.y / WorldSettings.GridCellRealWorldSize.y);
            var z = Mathf.FloorToInt(realWorldPos.z / WorldSettings.GridCellRealWorldSize.z);
            return new Vector3Int(x, y, z);
        }
        
        public bool IsCellExist(Vector3Int localGridPosition, out GridCell cachedCell)
        {
            return CachedCells.TryGetValue(localGridPosition, out cachedCell);
        }

        public GridCell CreateCell(Vector3Int cellPos)
        {
            var newCell = new GridCell(cellPos, CellPositionToRealWorld(cellPos));
            CachedCells.Add(cellPos, newCell);
            return newCell;

        }

        public bool IsPlacableSuitable(Vector3Int cellPos, Placable placable, float rotation, List<Vector3Int> bounds = null)
        {
            var requiredGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Required);
            foreach (var requiredCells in requiredGrids)
            {
                foreach (var placableRequiredCell in requiredCells.CellPositions)
                {
                    var checkingCellPos = cellPos + placableRequiredCell.RotateVector(rotation);

                    if (bounds != null && !bounds.Contains(cellPos))
                    {
                        return false;
                    }
                    
                    if (IsCellExist(checkingCellPos, out var cell) && cell.CellState is not CellState.CanBeFilled)
                    {
                        return false;
                    }
                }
            }

            var shouldPlaceOnGroundGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.ShouldPlaceOnGround);
            foreach (var shouldPlaceOnGroundGrid in shouldPlaceOnGroundGrids)
            {
                foreach (var placableShouldPlacedOnGroundGrid in shouldPlaceOnGroundGrid.CellPositions)
                {
                    var checkingCellPos =
                        cellPos + placableShouldPlacedOnGroundGrid.RotateVector(rotation);

                    if (bounds != null && !bounds.Contains(cellPos))
                    {
                        return false;
                    }
                    
                    if (IsCellExist(checkingCellPos, out var cell) && cell.CellLayer is not CellLayer.CanPlacableGround)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void AddItem(Placable placable, Vector3Int originPos, float rotation, CellLayer cellLayer)
        {
            var newGroundCells = new List<GridCell>();
            var physicalCells = new List<GridCell>();
            
            var physicalVolumes = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.PhysicalVolume);
            foreach (var physicalVolume in physicalVolumes)
            {
                foreach (var physicalCellPos in physicalVolume.CellPositions)
                {
                    var rotatedCellPos = originPos + physicalCellPos.RotateVector(rotation);
                    
                    if (IsCellExist(rotatedCellPos, out var cell))
                    {
                        cell.FillCell(placable, cellLayer);
                        physicalCells.Add(cell);
                    }
                    else
                    {
                        var newCell = CreateCell(rotatedCellPos);
                        newCell.FillCell(placable, cellLayer);
                        physicalCells.Add(newCell);
                    }
                }
            }

            var lockGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Lock);
            foreach (var lockGrid in lockGrids)
            {
                foreach (var placableLockCellPos in lockGrid.CellPositions)
                {
                    var rotatedCellPos = originPos + placableLockCellPos.RotateVector(rotation);

                    if (IsCellExist(rotatedCellPos, out var cell))
                    {
                        cell.LockCell();
                    }
                    else
                    {
                        var newCell = CreateCell(rotatedCellPos);
                        newCell.LockCell();
                    }
                }
            }

            var newGroundGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.NewGround);
            foreach (var newGroundGrid in newGroundGrids)
            {
                foreach (var placableNewGroundCellPos in newGroundGrid.CellPositions)
                {
                    var rotatedCellPos = originPos + placableNewGroundCellPos.RotateVector(rotation);
                    
                    if (IsCellExist(rotatedCellPos, out var cell))
                    {
                        cell.MakeCellCanBeFilledGround();
                        newGroundCells.Add(cell);
                    }
                    else
                    {
                        var newCell = CreateCell(rotatedCellPos);
                        newCell.MakeCellCanBeFilledGround();
                        newGroundCells.Add(newCell);
                    }
                } 
            }

            var placableGrids = new PlacableGrids(physicalCells, newGroundCells);
            ItemCellsDict.Add(placable, placableGrids);
        }

        public void DeleteItem(Placable placable)
        {
            var physicalCells = ItemCellsDict[placable].PhysicalCells;
            var newGroundCells = ItemCellsDict[placable].NewGroundCells;
            
            foreach (var physicalCell in physicalCells)
            {
                physicalCell.MakeCellEmpty();
                physicalCell.FreeTheCell();
            }
            
            foreach (var newGroundCell in newGroundCells)
            {
                newGroundCell.MakeCellEmpty();
                newGroundCell.FreeTheCell();
            }
        }
    }
}