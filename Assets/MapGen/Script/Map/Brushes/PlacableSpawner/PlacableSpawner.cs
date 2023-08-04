using System;
using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    
    [CreateAssetMenu(fileName = "Obstacle Brush", menuName = "MapGen/Brushes/Obstacle/Brush", order = 0)]
    public class PlacableSpawner : SingleCellEditableBrush
    {
        [SerializeField] private EndlessList<Placable> _placables;
        [SerializeField] private float _placableGizmoRadius = 0.25f;
        [SerializeField] private PlacableCellType _visualShownCellType;
        [SerializeField] private int _rotationStep = 15;
        
        public override string BrushName => "Placable";
        protected override int HitBrushHeight => 1;

        private List<Vector3Int> _placableVisuals = new();
        private Color _placableVisualColor = Color.green;
        private Color _cursorColor = Color.yellow;
        private int _rotation;
        
        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _placables.NextItem();   
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _placables.PreviousItem();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _rotation += _rotationStep;
                
                if (_rotation > 360)
                {
                    _rotation = 0;
                }
                else if (_rotation < 0)
                {
                    _rotation = 360;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _rotation -= _rotationStep;

                if (_rotation > 360)
                {
                    _rotation = 0;
                }
                else if (_rotation < 0)
                {
                    _rotation = 360;
                }
            }
            
            var grid = _placables.CurrentItem.Grids.FirstOrDefault(grid => grid.PlacableCellType == _visualShownCellType);
            if (grid == null) return;

            _placableVisuals = grid.TransformAccordingToSpawn(_placables.CurrentItem, HitPosOffsetted, _rotation);
            
            
            if (WorldCreator.Grid.IsPlacableSuitable(HitPosOffsetted, _placables.CurrentItem, _rotation))
            {
                _placableVisualColor = Color.green;
            }
            else
            {
                _placableVisualColor = Color.red;
            }
            
        }

        public override void OnDrawGizmos()
        { 
            base.OnDrawGizmos();
            
            
            Gizmos.color = _cursorColor;
            var cursor = WorldCreator.Grid.CellPositionToRealWorld(HitPosOffsetted);
            Gizmos.DrawWireCube(cursor, Vector3.one);
            
            Gizmos.color = _placableVisualColor;
            foreach (var placableVisual in _placableVisuals)
            {
                var pos = WorldCreator.Grid.CellPositionToRealWorld(placableVisual);
                Gizmos.DrawSphere(pos, _placableGizmoRadius);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }

        public override ICommand GetPaintCommand(Vector3Int startPoint, Grid grid)
        {
            return new PlacableSpawnerCommand(this, WorldCreator.Instance, startPoint, grid);
        }

        public SpawnData? Paint(Vector3Int startPoint, Grid grid)
        {
            if (!WorldCreator.Instance.Grid.IsPlacableSuitable(startPoint, _placables.CurrentItem, _rotation)) return null;

            var data = new SpawnData(startPoint, _placables.CurrentItem, _rotation, CellLayer.Obstacle);
            WorldCreator.Instance.SpawnObject(data);

            return data;
        }
    }
}