﻿using System;
using System.Collections.Generic;
using MapGen.Map;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using Weaver;

namespace MapGen.GridSystem
{
    public class Grid
    {
        public Dictionary<Placable, PlacableGrids> ItemCellsDict { get; }
        public Dictionary<Placable, GridCell> ItemOriginsDict { get; }

        public Dictionary<Vector3Int, GridCell> CachedCells { get; } = new();

        public Grid()
        {
            ItemCellsDict = new Dictionary<Placable, PlacableGrids>();
            ItemOriginsDict = new Dictionary<Placable, GridCell>();
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
            var x = Mathf.RoundToInt(realWorldPos.x / WorldSettings.GridCellRealWorldSize.y);
            var y = Mathf.RoundToInt(realWorldPos.y / WorldSettings.GridCellRealWorldSize.y);
            var z = Mathf.RoundToInt(realWorldPos.z / WorldSettings.GridCellRealWorldSize.z);
            
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

        public bool IsPlacableSuitable(Vector3Int cellPos, Placable placable, int rotation, List<Vector3Int> bounds = null)
        {
            var requiredGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Required);
            foreach (var requiredCells in requiredGrids)
            {
                foreach (var placableRequiredCell in requiredCells.CellPositions)
                {
                    var checkingCellPos = cellPos + placableRequiredCell.RotateVector(rotation, placable.Origin);

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
                        cellPos + placableShouldPlacedOnGroundGrid.RotateVector(rotation, placable.Origin);

                    if (bounds != null && !bounds.Contains(cellPos))
                    {
                        return false;
                    }
                    
                    if (!IsCellExist(checkingCellPos, out var cell))
                    {
                        return false;
                    }

                    if (cell.CellLayer is not CellLayer.CanPlacableGround)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void AddItem(Placable placable, Vector3Int originPos, int rotation, CellLayer cellLayer)
        {
            var placableGrids = new PlacableGrids();

            if(!IsCellExist(originPos, out var originCell))
            {
                originCell = CreateCell(originPos);
            }

            originCell.OriginatedItems.Add(placable);
            ItemOriginsDict.Add(placable, originCell);
            
            var physicalVolumes = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.PhysicalVolume);
            foreach (var physicalVolume in physicalVolumes)
            {
                foreach (var physicalCellPos in physicalVolume.CellPositions)
                {
                    var rotatedCellPos = originPos + physicalCellPos.RotateVector(rotation, placable.Origin);
                    
                    if (!IsCellExist(rotatedCellPos, out var cell))
                    {
                        cell = CreateCell(rotatedCellPos);
                    }
                    
                    cell.FillCell(placable, cellLayer);
                    placableGrids.Add(PlacableCellType.PhysicalVolume, cell);
                }
            }

            var lockGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Lock);
            foreach (var lockGrid in lockGrids)
            {
                foreach (var placableLockCellPos in lockGrid.CellPositions)
                {
                    var rotatedCellPos = originPos + placableLockCellPos.RotateVector(rotation, placable.Origin);

                    if (!IsCellExist(rotatedCellPos, out var cell))
                    {
                        cell = CreateCell(rotatedCellPos);
                    }
                    
                    cell.LockCell();

                }
            }

            var newGroundGrids = placable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.NewGround);
            foreach (var newGroundGrid in newGroundGrids)
            {
                foreach (var placableNewGroundCellPos in newGroundGrid.CellPositions)
                {
                    var rotatedCellPos = originPos + placableNewGroundCellPos.RotateVector(rotation, placable.Origin);
                    
                    if (!IsCellExist(rotatedCellPos, out var cell))
                    {
                        cell = CreateCell(rotatedCellPos);
                    }

                    if (cell.Item == null)
                    {
                        cell.MakeCellCanBeFilledGround();
                        placableGrids.Add(PlacableCellType.NewGround, cell);
                    }
                } 
            }

            ItemCellsDict.Add(placable, placableGrids);
        }

        public void DeleteItem(Placable placable)
        {
            var physicalCells = ItemCellsDict[placable].Cells[PlacableCellType.PhysicalVolume];
            var newGroundCells = ItemCellsDict[placable].Cells[PlacableCellType.NewGround];

            if (ItemOriginsDict.ContainsKey(placable))
            {
                ItemOriginsDict[placable].OriginatedItems.Remove(placable);
            }
            
            foreach (var physicalCell in physicalCells)
            {
                physicalCell.MakeCellEmpty();
                physicalCell.FreeTheCell();
            }
            
            foreach (var newGroundCell in newGroundCells)
            {
                if (newGroundCell.Item == null)
                {
                    newGroundCell.MakeCellEmpty();
                    newGroundCell.FreeTheCell();;
                }
            }

            ItemCellsDict.Remove(placable);
        }

        public void RegenerateShouldPlaceOnGrounds()
        {
            foreach (var (placable, placableGrids) in ItemCellsDict)
            {
                foreach (var placableGridsShouldPlacedOnCell in placableGrids.Cells[PlacableCellType.NewGround])
                {
                    if (placableGridsShouldPlacedOnCell.Item != null)
                    {
                        continue;
                    }
                    
                    placableGridsShouldPlacedOnCell.MakeCellCanBeFilledGround();
                }
            }
        }
    }
}