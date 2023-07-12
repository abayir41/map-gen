using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map.Brushes;
using MapGen.Placables;
using UnityEngine;

namespace MapGen.Map
{
    public class WorldCreator : MonoBehaviour
    {
        private const int MAP_BRUSH_TARGETED_SELECTABLE_CELL_Y_AXIS = 0;
        
        public static WorldCreator Instance { get; private set; }

        [SerializeField] private WorldSettings _worldSettings;
        [SerializeField] private Transform _gridPrefabsParent;
        [SerializeField] private Transform _mapBrushTargetedSelectableCellParent;
        [SerializeField] private SelectableGridCell _mapBrushTargetedSelectableCell;
        [SerializeField] private Brush _currentBrush;
        
        public Transform GridPrefabsParent => _gridPrefabsParent;
        public Brush CurrentBrush => _currentBrush;
        public WorldSettings WorldSettings => _worldSettings;

        public Grid Grid { get; private set; }
        private GridHelper _gridHelper;


        private void Awake()
        {
            Instance = this;
            Grid = new Grid(_worldSettings.MapSize);
            _gridHelper = new GridHelper(Grid);
            InstantiateMapBrushTargetedSelectableCells();
        }

        private void InstantiateMapBrushTargetedSelectableCells()
        {
            var cells = Grid.GetYAxisOfGrid(MAP_BRUSH_TARGETED_SELECTABLE_CELL_Y_AXIS);
            
            foreach (var gridCell in cells)
            {
                var selectableGridCell = Instantiate(_mapBrushTargetedSelectableCell, _mapBrushTargetedSelectableCellParent);
                selectableGridCell.transform.position = Grid.GetWorldPosition(gridCell);
                selectableGridCell.BoundedCell = gridCell;
            }
        }

        public void PaintTheBrush(List<GridCell> selectedCells)
        {
            DestroyPlacedPlacables(selectedCells);
            _currentBrush.Paint(selectedCells, Grid);
        }
        
        private void DestroyPlacedPlacables(List<GridCell> selectedCells)
        {
            foreach (var selectedCell in selectedCells)
            {
                if (selectedCell.PlacedItem == null) continue;
                DestroyItem(selectedCell.PlacedItem);
            }
        }
        
        public void DestroyItem(Placable placable)
        {
            foreach (var gridElement in Grid.Cells)
            {
                if (gridElement.PlacedItem == placable)
                {
                    gridElement.FreeTheCell();
                    gridElement.MakeCellEmpty();
                }
            }

            DestroyPlacable(placable);
        }

        private void DestroyPlacable(Placable item)
        {
            Destroy(item.gameObject);
        }
        
        public Placable SpawnObject(GridCell pos, Placable placable, CellLayer cellLayer, float rotation, string objName = null)
        {
            if (!_gridHelper.IsPlacableSuitable(pos, placable, rotation))
            {
                Debug.LogWarning($"Trying to spawn an object that is not suitable. Process Canceled. Position: {pos.Position}");
                return null;
            }

            var instantiatedPlacable = Instantiate(placable, WorldCreator.Instance.GridPrefabsParent);
            instantiatedPlacable.InitializePlacable(pos.Position);
            instantiatedPlacable.transform.position = pos.GetWorldPosition();

            if (objName != null)
                instantiatedPlacable.name = objName;
                
            if (instantiatedPlacable.Rotatable)
            {
                instantiatedPlacable.Rotate(rotation);
            }

            var physicalVolumes = instantiatedPlacable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.PhysicalVolume);
            foreach (var physicalVolume in physicalVolumes)
            {
                foreach (var physicalCellPos in physicalVolume.CellPositions)
                {
                    var checkedGridPos = pos.Position + GridHelper.RotateObstacleVector(rotation, physicalCellPos);
                    var cell = Grid.GetCell(new Vector3Int(checkedGridPos.x, checkedGridPos.y, checkedGridPos.z));
                    cell.FillCell(instantiatedPlacable, cellLayer);
                }
            }

            var lockGrids = instantiatedPlacable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.Lock);
            foreach (var lockGrid in lockGrids)
            {
                foreach (var placableLockGrid in lockGrid.CellPositions)
                {
                    var checkedGridPos = pos.Position + GridHelper.RotateObstacleVector(rotation, placableLockGrid);
                    if (_gridHelper.IsPosOutsideOfGrid(checkedGridPos))
                        continue;

                    var cell = Grid.GetCell(new Vector3Int(checkedGridPos.x, checkedGridPos.y, checkedGridPos.z));

                    if (cell.CellState != CellState.Filled)
                        cell.LockCell();
                }
            }

            var newGroundGrids = instantiatedPlacable.Grids.FindAll(grid => grid.PlacableCellType == PlacableCellType.NewGround);
            foreach (var newGroundGrid in newGroundGrids)
            {
                foreach (var placableNewGroundGrid in newGroundGrid.CellPositions)
                {
                    var checkedGridPos =
                        pos.Position + GridHelper.RotateObstacleVector(rotation, placableNewGroundGrid);
                    if (_gridHelper.IsPosOutsideOfGrid(checkedGridPos))
                        continue;

                    var cell = Grid.GetCell(new Vector3Int(checkedGridPos.x, checkedGridPos.y, checkedGridPos.z));

                    if (cell.CellState is not (CellState.Locked or CellState.Filled))
                        cell.MakeCellCanBeFilledGround();
                } 
            }

            return instantiatedPlacable;
        }
    }
}