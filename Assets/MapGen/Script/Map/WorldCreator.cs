﻿using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map.Brushes;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map
{
    public class WorldCreator : MonoBehaviour
    {
        public static WorldCreator Instance { get; private set; }
        
        [SerializeField] private WorldSettings _worldSettings;
        [SerializeField] private Transform _gridPrefabsParent;
        [SerializeField] private Transform _selectableGridCellParent;
        [SerializeField] private SelectableGridCell _selectableGridCell;

        private Dictionary<Placable, HashSet<SelectableGridCell>> _placablePhysicals = new(); 

        public Grid Grid { get; private set; }

        private void Awake()
        {
            Instance = this;
            Grid = new Grid();
        }

        public void DestroyByCellPoint(Vector3Int pos)
        {
            if (!Grid.IsCellExist(pos, out var cell))
            {
                Debug.Log("Cell Does not exist: " + pos);
                return;
            }

            if (cell.Item == null)
            {
                Debug.Log("Cell item is null: " + pos);
                return;
            }
            
            DestroyItem(cell.Item);
        }
        
        public void DestroyItem(Placable placable)
        {
            Grid.DeleteItem(placable);
            foreach (var selectableGridCell in _placablePhysicals[placable])
            {
                DestroyPhysicalSelectable(selectableGridCell);
            }
            DestroyPlacable(placable);
        }

        private void DestroyPhysicalSelectable(SelectableGridCell selectableGridCell)
        {
            Destroy(selectableGridCell.gameObject);
        }
        private void DestroyPlacable(Placable item)
        {
            Destroy(item.gameObject);
        }

        public void SpawnObject(SpawnData data)
        {
            var instantiatedPlacable = Instantiate(data.Prefab, _gridPrefabsParent);
            instantiatedPlacable.InitializePlacable(data);

            var realPos = data.SpawnPos - data.Prefab.Origin;
            instantiatedPlacable.transform.position = Grid.CellPositionToRealWorld(realPos);

            if (data.ObjName != null)
                instantiatedPlacable.name = data.ObjName;
                
            if (instantiatedPlacable.Rotatable)
            {
                instantiatedPlacable.Rotate(data.Rotation);
            }
            
            Grid.AddItem(instantiatedPlacable, data.SpawnPos, data.Rotation, data.CellLayer);
            var physicalGrid =
                instantiatedPlacable.Grids.FirstOrDefault(grid =>
                    grid.PlacableCellType == PlacableCellType.PhysicalVolume);

            _placablePhysicals.Add(instantiatedPlacable, new HashSet<SelectableGridCell>());
            
            
            if (physicalGrid != null)
            {
                var transformedGrid = physicalGrid.CellPositions.ConvertAll(input => input.RotateVector(data.Rotation, data.Prefab.Origin));
                transformedGrid = transformedGrid.ConvertAll(input => input + data.SpawnPos);
                
                foreach (var physicalGridCellPosition in transformedGrid)
                {
                    var worldPos = Grid.CellPositionToRealWorld(physicalGridCellPosition);
                    var selectableGridCell = Instantiate(_selectableGridCell, _selectableGridCellParent);
                    selectableGridCell.transform.position = worldPos;

                    if (!Grid.IsCellExist(physicalGridCellPosition, out var cell))
                    {
                        cell = Grid.CreateCell(physicalGridCellPosition);
                    }

                    selectableGridCell.BoundedCell = cell;
                    selectableGridCell.BoundedPlacable = instantiatedPlacable;
                    
                    _placablePhysicals[instantiatedPlacable].Add(selectableGridCell);
                }
            }
        }
    }
}