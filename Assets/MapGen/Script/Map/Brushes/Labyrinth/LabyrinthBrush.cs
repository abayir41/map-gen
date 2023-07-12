using System.Collections.Generic;
using System.Linq;
using LabGen.Labyrinth;
using LabGen.Placables;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushHelper;
using MapGen.Placables;
using Maze;
using UnityEngine;

namespace MapGen.Map.Brushes
{
    [CreateAssetMenu(fileName = "Labyrinth Brush", menuName = "MapGen/Brushes/Labyrinth/Brush", order = 0)]
    public class LabyrinthBrush : Brush
    {
        [SerializeField] private LabyrinthBrushSettings _labyrinthBrushSettings;

        public LabyrinthBrushSettings LabyrinthBrushSettings => _labyrinthBrushSettings;

        private SelectedCellsHelper _selectedCellsHelper;

        public override List<Placable> Paint(List<GridCell> selectedCells, Grid grid)
        {
            var result = new List<Placable>();
            _selectedCellsHelper = new SelectedCellsHelper(selectedCells, grid);

            var labyrinthPieceSize = _labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness;
            var labyrinthPieceAmount = new Vector2Int((_selectedCellsHelper.XWidth - labyrinthPieceSize) / labyrinthPieceSize,
                (_selectedCellsHelper.ZWidth - labyrinthPieceSize) / labyrinthPieceSize);
            var maze = MazeGenerator.Generate(labyrinthPieceAmount.x, labyrinthPieceAmount.y, _labyrinthBrushSettings.RandomSettings.GetSeed());
            
            Debug.Log("Size: " + labyrinthPieceSize);
            Debug.Log("Piece: " + labyrinthPieceAmount);
            
            for (int x = 0; x < labyrinthPieceAmount.x; x++)
            {
                for (int z = 0; z < labyrinthPieceAmount.y; z++)
                {
                    var startPoint =
                        new Vector2Int(_selectedCellsHelper.MinX + x * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness),
                            _selectedCellsHelper.MinZ + z * (_labyrinthBrushSettings.WallThickness + _labyrinthBrushSettings.WayThickness));
                    
                    var labyrinthBrushHelper = new LabyrinthBrushHelper(startPoint, _labyrinthBrushSettings.WallThickness,
                        _labyrinthBrushSettings.WayThickness, _labyrinthBrushSettings.WallHeight);
                    
                    
                    var cell = maze[x, z];
                    if (cell.HasFlag(WallState.Up))
                    {
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Top, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight, result));
                    }

                    if (cell.HasFlag(WallState.Left))
                    {
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopLeft, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Left, result));
                        result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, result));
                    }

                    if (x == labyrinthPieceAmount.x - 1)
                    {
                        if (cell.HasFlag(WallState.Right))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Right, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.TopRight, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight, result));
                        }
                    }

                    if (z == 0)
                    {
                        if (cell.HasFlag(WallState.Down))
                        {
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.Bottom, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomRight, result));
                            result.AddRange(OpenWallIfNecessary(labyrinthBrushHelper, MazeCubicPositions.BottomLeft, result));
                        }
                    }
                }
            }

            return result;
        }

        public List<Placable> OpenWallIfNecessary(LabyrinthBrushHelper brushHelper, MazeCubicPositions position, List<Placable> alreadyPlacedPlacables)
        {
            var positions = brushHelper.PosVisualsDict[position];
            var result = new List<Placable>();
            foreach (var pos in positions)
            {
                if(alreadyPlacedPlacables.Any(item => item.GridPos == pos)) continue;
                var cell = _selectedCellsHelper.GetCell(pos);
                cell.MakeCellCanBeFilledGround();
                var placable = WorldCreator.Instance.SpawnObject(cell, _labyrinthBrushSettings.MazeCubicGridPlacable, CellLayer.Ground,
                    LabyrinthBrushSettings.GROUND_ROTATION, position.ToString());
                result.Add(placable);
            }

            return result;
        }

        private bool IsOutsideOfMap<T>(Vector2Int pos, T[,] wallStates)
        {
            return IsOutsideOfMap(pos.x, pos.y, wallStates);
        }
        
        private bool IsOutsideOfMap<T>(int x, int y, T[,] wallStates)
        {
            return x < 0 || x >= wallStates.GetLength(0) || y < 0 || y >= wallStates.GetLength(1);
        }
    }
}