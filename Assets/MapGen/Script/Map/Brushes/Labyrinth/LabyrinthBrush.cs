using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using Maze;
using Unity.VisualScripting;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes.Labyrinth
{
    [CreateAssetMenu(fileName = "Labyrinth Brush", menuName = "MapGen/Brushes/Labyrinth/Brush", order = 0)]
    public class LabyrinthBrush : Brush
    {
        [SerializeField] private LabyrinthBrushSettings _labyrinthBrushSettings;

        public LabyrinthBrushSettings LabyrinthBrushSettings => _labyrinthBrushSettings;

        
        private SelectedCellsHelper _selectedCellsHelper;
        private Grid _grid;
        private List<Vector3Int> _groundCells;

        public override List<Placable> Paint(List<Vector3Int> selectedCells, Grid grid)
        {
            
            var result = new List<Placable>();
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);
            _groundCells = _selectedCellsHelper.GetYAxisOfGrid(LabyrinthBrushSettings.GROUND_Y_LEVEL);
            _grid = grid;

            result.AddRange(CreateGround());
            
            var labyrinthPieceSize = _labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness;
            var labyrinthPieceAmount = new Vector2Int((_selectedCellsHelper.XWidth - labyrinthPieceSize) / labyrinthPieceSize,
                (_selectedCellsHelper.ZWidth - labyrinthPieceSize) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, _labyrinthBrushSettings.RandomSettings.GetSeed());
            
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    var startPoint =
                        new Vector2Int(_selectedCellsHelper.MinX + x * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness),
                            _selectedCellsHelper.MinZ + z * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushHelper(startPoint, _labyrinthBrushSettings.WallThickness,
                        _labyrinthBrushSettings.WayThickness, _labyrinthBrushSettings.WallHeight, LabyrinthBrushSettings.LABYRINTH_START_Y_LEVEL);
                    
                    
                    var cell = maze[x, z];

                    WallState? leftBottomWallState = null;
                    if (x > 0 && z > 0)
                    {
                        leftBottomWallState = maze[x - 1, z - 1];
                    }
                    WallState? bottomWallState = null;
                    if (z > 0)
                    {
                        bottomWallState = maze[x, z - 1];
                    }
                    WallState? leftWallState = null;
                    if (x > 0)
                    {
                        leftWallState = maze[x - 1, z];
                    }
                    

                    var up = cell.HasFlag(WallState.Up);
                    var left = cell.HasFlag(WallState.Left);
                    var right = cell.HasFlag(WallState.Right);
                    var bottom = cell.HasFlag(WallState.Down);
                    
                    if (up)
                    {
                        if (leftWallState == null || !leftWallState.HasFlag(WallState.Up))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft));
                        }
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Top));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight));
                    }

                    if (left)
                    {
                        if (!up && (leftWallState == null || !leftWallState.HasFlag(WallState.Up)))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft));
                        }

                        if ((leftBottomWallState == null || !leftBottomWallState.HasFlag(WallState.Up)) &&
                            (bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) && 
                            (bottomWallState == null || !bottomWallState.HasFlag(WallState.Left)))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft));
                        }
                        
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Left));
                    }

                    if (x == labyrinthPieceAmount.x - 1)
                    {
                        if (right)
                        {
                            if (!up)
                            {
                                result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight));
                            }
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Right));

                            if ((bottomWallState == null || !bottomWallState.HasFlag(WallState.Up)) && (bottomWallState == null || !bottomWallState.HasFlag(WallState.Right)))
                            {
                                result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight));
                            }
                        }
                    }

                    if (z == 0)
                    {
                        if (bottom)
                        {
                            if (!right)
                            {
                                result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight));
                            }

                            if (!left && (leftWallState == null || !leftWallState.HasFlag(WallState.Down)))
                            {
                                result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft));
                            }
                            
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Bottom));
                        }
                    }
                }
            }
            
            return result.ToList();
        }
        
        private List<Placable> CreateGround()
        {
            var result = new List<Placable>();
            foreach (var selectedCell in _groundCells)
            {
                if (_grid.IsCellExist(selectedCell, out var cell))
                {
                    cell.MakeCellCanBeFilledGround();
                }
                var placable =  WorldCreator.Instance.SpawnObject(selectedCell, _labyrinthBrushSettings.Ground, CellLayer.Ground, LabyrinthBrushSettings.GROUND_ROTATION);
                result.Add(placable);
            }

            return result;
        }

        
        
        public List<Placable> OpenWallIfNecessary(LabyrinthBrushHelper brushHelper, MazeCubicPositions position)
        {
            var result = new List<Placable>();
            
            var positions = brushHelper.PosVisualsDict[position];
            
            foreach (var pos in positions)
            {
                if (_grid.IsCellExist(pos, out var cell))
                {
                    cell.MakeCellCanBeFilledGround();
                }
                
                var placable = WorldCreator.Instance.SpawnObject(pos, _labyrinthBrushSettings.MazeCubicGridPlacable, CellLayer.Ground,
                    LabyrinthBrushSettings.GROUND_ROTATION,position.ToString());
                result.Add(placable);
                
            }
            
            return result;
        }
    }
}