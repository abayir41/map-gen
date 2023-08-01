using System.Collections.Generic;
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

        public Placable SpawnObject(Vector3Int pos, Placable placable, CellLayer cellLayer, int rotation, string objName = null)
        {
            var instantiatedPlacable = Instantiate(placable, _gridPrefabsParent);
            instantiatedPlacable.InitializePlacable(pos);

            var realPos = pos - placable.Origin;
            instantiatedPlacable.transform.position = Grid.CellPositionToRealWorld(realPos);

            if (objName != null)
                instantiatedPlacable.name = objName;
                
            if (instantiatedPlacable.Rotatable)
            {
                instantiatedPlacable.Rotate(rotation);
            }
            
            Grid.AddItem(instantiatedPlacable, pos, rotation, cellLayer);
            var physicalGrid =
                instantiatedPlacable.Grids.FirstOrDefault(grid =>
                    grid.PlacableCellType == PlacableCellType.PhysicalVolume);

            _placablePhysicals.Add(instantiatedPlacable, new HashSet<SelectableGridCell>());
            
            
            if (physicalGrid != null)
            {
                var transformedGrid = physicalGrid.CellPositions.ConvertAll(input => input.RotateVector(rotation, placable.Origin));
                transformedGrid = transformedGrid.ConvertAll(input => input + pos);
                
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
            
            return instantiatedPlacable;
        }
    }
}